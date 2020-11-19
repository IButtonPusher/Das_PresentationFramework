using Das.Views;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AsyncResults.ForEach;
using Das.Views.Mvvm;

namespace Das.ViewModels
{
    public class AsyncObservableCollection<T> : InvokableViewModel, 
                                                IList,
                                                IAsyncReadOnlyCollection<T>,
                                                IAsyncObservableCollection<T>,
                                                IReadOnlyList<T>
        where T : IEquatable<T>
    {
        public AsyncObservableCollection(ISingleThreadedInvoker ui)
            : this(Enumerable.Empty<T>(), ui)
        {
        }

        public AsyncObservableCollection(IEnumerable<T> items, ISingleThreadedInvoker ui)
            : base(ui)
        {
            _ui = ui;
            _hashCheck = new HashSet<T>(items);
            _addedInTransaction = new ConcurrentQueue<T>();
            _removedInTransaction = new ConcurrentQueue<T>();
            _backingCollection = ui.Invoke(() =>
            {
                var bc = new ObservableCollection<T>(_hashCheck);
                Count = bc.Count;
                bc.CollectionChanged += OnBackingCollectionChanged;
                INotifyPropertyChanged exp = bc;
                exp.PropertyChanged += OnBackingCollectionPropertyChanged;

                return bc;
            });
        }

        public AsyncObservableCollection(ISingleThreadedInvoker ui, params IEnumerable<T>[] itemses)
            : this(SelectFromMany(itemses), ui)
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            CollectionChanged = null;
            _backingCollection.CollectionChanged -= OnBackingCollectionChanged;

            foreach (var bc in _backingCollection.OfType<IDisposable>())
                bc.Dispose();

            INotifyPropertyChanged exp = _backingCollection;
            exp.PropertyChanged -= OnBackingCollectionPropertyChanged;

            _backingCollection.Clear();
            Count = 0;
        }

        public async IAsyncEnumerator<T> GetAsyncEnumerator(
#pragma warning disable 8424
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
#pragma warning restore 8424
        {
            foreach (var item in await ToArrayAsync())
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;
                yield return item;
            }
        }

        //private IEnumerable<Boolean> AddRangeImpl(IEnumerable<T> items)
        //{
        //    foreach (var item in items)
        //        yield return AddImpl(item);
        //}

        public async Task RunOnEach(Action<T> action)
        {
            await foreach (var t in this)
                action(t);
        }

        public async IAsyncEnumerable<TRes> GetFromEach<TRes>(Func<T, TRes> action)
        {
            await foreach (var i in this)
                yield return action(i);
        }

        public async IAsyncEnumerable<T> EnumerateAsync()
        {
            var arr = await GetEnumerableAsync();

            foreach (var r in arr)
                yield return r;
        }

        public Boolean Remove(T item)
        {
            return _ui.Invoke(() => RemoveImpl(item));
        }

        T IAsyncObservableCollection<T>.this[Int32 index]
            => _ui.Invoke(() => _backingCollection[index]);

        public async Task<Boolean> AddAsync(T item)
        {
            return await _ui.InvokeAsync(() => AddImpl(item)).ConfigureAwait(false);
        }

        public async Task AddAsync(IEnumerable<T> items)
        {
            await DoTransaction(k =>
            {
                foreach (var item in items)
                    AddImpl(item);
            }).ConfigureAwait(false);
        }


        //public void AddRange(IEnumerable<T> items)
        //{
        //    DoBlockingTransaction(c =>
        //    {
        //        foreach (var item in items)
        //            AddImpl(item);
        //    });
        //}

        public async Task Synchronize(IEnumerable<T> items)
        {
            await DoTransaction(k =>
            {
                var itemSearch = new HashSet<T>(items);
                var remove = _backingCollection.Where(b => !itemSearch.Contains(b)).ToArray();
                for (var c = 0; c < remove.Length; c++)
                {
                    _count--;
                    _backingCollection.Remove(remove[c]);
                    _hashCheck.Remove(remove[c]);
                }

                var add = itemSearch.Where(b => !_hashCheck.Contains(b)).ToArray();
                for (var c = 0; c < add.Length; c++)
                {
                    _count++;
                    _backingCollection.Add(add[c]);
                    _hashCheck.Add(add[c]);
                }
            });

            await OnCollectionReset().ConfigureAwait(false);
        }

        public async Task Synchonize<TOther>(NotifyCollectionChangedEventArgs args,
                                             Func<T, TOther, Boolean> equate,
                                             Func<TOther, T> forNew)
        {
            await DoAsyncTransaction(async k =>
            {
                if (args.OldItems is { } goneNow)
                {
                    var going = goneNow.OfType<TOther>().ToArray();

                    var rip = await SelectAsync(u => going.Any(g => equate(u, g)));
                    await TryRemove(rip);
                }

                if (args.NewItems is IList someList)
                {
                    var addMany = new List<T>();

                    foreach (var added in someList.OfType<TOther>())
                    {
                        var letsAdd = forNew(added);
                        addMany.Add(letsAdd);
                    }

                    await AddAsync(addMany);
                }
            }).ConfigureAwait(false);
        }

        public async Task Synchonize<TOther>(IEnumerable<TOther> args,
                                             Func<T, TOther, Boolean> equate,
                                             Func<TOther, T> forNew)
        {
            var others = new List<T>(args.Select(forNew));
            await Synchronize(others).ConfigureAwait(false);
        }


        public async Task<T[]> ToArrayAsync()
        {
            var itar = await GetEnumerableAsync();
            return itar.ToArray();
        }


        public async Task DoAsyncTransaction(Func<IAsyncObservableCollection<T>, Task> action)
        {
            var args = await _ui.InvokeAsync(() => DoTransactionImpl(() => action(this)));
            if (args == null)
                return;

            await NotifyCollectionChangedAsync(args).ConfigureAwait(false);
        }

        public async Task DoTransaction(Action<IAsyncObservableCollection<T>> action)
        {
            await DoTransactionCore(() => action(this)).ConfigureAwait(false);
        }

        public async Task DoTransaction<TInput>(IEnumerable<TInput> datas,
                                                Action<IAsyncObservableCollection<T>, TInput> action)
        {
            var ds = datas.ToArray();

            void LetsRun()
            {
                foreach (var d in ds!)
                    action(this, d);
            }

            await DoTransactionCore(LetsRun).ConfigureAwait(false);
        }

        public async Task DoTransaction<TInput>(IAsyncEnumerable<TInput> datas,
                                                Action<IAsyncObservableCollection<T>, TInput> action)
        {
            var ds = await datas.ToArrayAsync();

            void LetsRun()
            {
                foreach (var d in ds!)
                    action(this, d);
            }

            await DoTransactionCore(LetsRun).ConfigureAwait(false);
        }


        public async Task AddRangeAsync(IEnumerable<T> items)
        {
            await DoTransaction(items, (_, item) => { AddImpl(item); }).ConfigureAwait(false);
        }

        public async Task AddRangeAsync(IAsyncEnumerable<T> items)
        {
            await DoTransaction(items, (_, item) => { AddImpl(item); }).ConfigureAwait(false);
        }

        public async Task<Boolean> ContainsAsync(T item)
        {
            return await _ui.InvokeAsync(() => _backingCollection.Contains(item));
        }

        public async Task<Int32> IndexOfAsync(T item)
        {
            return await _ui.InvokeAsync(() => _backingCollection.IndexOf(item));
        }

        public async Task<Boolean> AddOrUpdateAsync(T item)
        {
            if (await AddAsync(item))
                return true;

            await _ui.InvokeAsync(() =>
            {
                _hashCheck.Remove(item);
                _hashCheck.Add(item);
                var updating = _backingCollection.FirstOrDefault(b => b.Equals(item));

                _backingCollection.Remove(updating);
                _backingCollection.Add(item);
            }).ConfigureAwait(false);

            return false;
        }

        public async Task<Boolean> TryRemoveAsync(T item)
        {
            return await _ui.InvokeAsync(() => RemoveImpl(item));
        }


        public async Task TryRemove(IEnumerable<T> items)
        {
            var arr = new List<T>(items);

            await DoTransaction(arr, (a, b) => { RemoveImpl(b); }).ConfigureAwait(false);
        }


        public async Task<Boolean> TryRemove(ICollection<T> items,
                                             IProgress<T> progress)
        {
            var arr = new List<T>(items);

            return await DoTransaction(arr, (a, b) =>
            {
                if (RemoveImpl(b))
                    progress.Report(b);
                else return false;

                return true;
            });
        }


        public async Task Remove(IEnumerable<T> items)
        {
            await DoTransaction(k =>
            {
                foreach (var item in items)
                {
                    if (!_hashCheck.Remove(item))
                        continue;

                    _count--;
                    _backingCollection.Remove(item);
                }
            }).ConfigureAwait(false);
        }

        public async Task ClearAsync()
        {
            if (Count == 0)
                return;

            await DoTransaction(_ => ClearImpl()).ConfigureAwait(false);
            await OnCollectionReset().ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<T>> SelectAsync(Func<T, Boolean> predicate)
        {
            var got = await _ui.InvokeAsync(() =>
            {
                var res = GetNewReadOnlyList();

                foreach (var item in _backingCollection)
                    if (predicate(item))
                        res.Add(item);

                return res;
            });
            return got;
        }

        #if NET40
        private static ListEx<T> GetNewReadOnlyList() => new ListEx<T>();
        private static ListEx<T> GetNewReadOnlyList(IEnumerable<T> values) => new ListEx<T>(values);
                
        #else
        private static List<T> GetNewReadOnlyList() => new List<T>();
        private static List<T> GetNewReadOnlyList(IEnumerable<T> values) => new List<T>(values);
        
        #endif

        public async Task<IReadOnlyList<T>> SelectAsync<TArg>(Func<T, TArg, Boolean> predicate, TArg data)
        {
            var got = await _ui.InvokeAsync(() =>
            {
                var res = GetNewReadOnlyList();

                foreach (var item in _backingCollection)
                    if (predicate(item, data))
                        res.Add(item);

                return res;
            });
            return got;
        }

        public async Task<TCollection> SelectAsync<TCollection, TOut>(Func<T, Boolean> predicate,
                                                                      Func<T, TOut> selecting)
            where TCollection : ICollection<TOut>, new()
        {
            return await _ui.InvokeAsync(() =>
            {
                var items = _backingCollection.Where(predicate);
                var res = new TCollection();
                foreach (var i in items)
                {
                    var shallAdd = selecting(i);
                    res.Add(shallAdd);
                }

                return res;
            });
        }

        public async Task<T> FirstOrDefaultAsync()
        {
            if (Count == 0)
                return default!;
            var got = await _ui.InvokeAsync(() =>
            {
                if (_backingCollection.Count == 0)
                    return default!;

                return _backingCollection[0];
            });
            return got;
        }

        public async Task<T> FirstOrDefaultAsync(Func<T, Boolean> predicate)
        {
            var got = await _ui.InvokeAsync(() =>
            {
                foreach (var b in _backingCollection)
                    if (predicate(b))
                        return b;

                return default!;
            });
            return got;
        }

        public async Task<Boolean> AnyAsync(Func<T, Boolean> predicate)
        {
            var got = await _ui.InvokeAsync(() => _backingCollection.Any(predicate));
            return got;
        }

        public async Task<Boolean> AllAsync(Func<T, Boolean> predicate)
        {
            var got = await _ui.InvokeAsync(() => _backingCollection.All(predicate));
            return got;
        }


        public async Task<IReadOnlyList<T>> GetEnumerableAsync()
        {
            var got = await _ui.InvokeAsync(() => GetNewReadOnlyList(_backingCollection));
            return got;
        }


        public void Add(T item)
        {
            _ui.Invoke(() => { AddImpl(item); });
        }

        public Boolean Contains(T item)
        {
            return _ui.Invoke(() => _backingCollection.Contains(item));
        }

        public void CopyTo(T[] array, Int32 arrayIndex)
        {
            _ui.Invoke(() => { _backingCollection.CopyTo(array, arrayIndex); });
        }


        public async Task RemoveAtAsync(Int32 index)
        {
            await _ui.InvokeAsync(() => { RemoveAtImpl(index); }).ConfigureAwait(false);
        }

        public async Task InsertAsync(Int32 index, T item)
        {
            await _ui.InvokeAsync(() => InsertImpl(index, item)).ConfigureAwait(false);
        }


        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        T IAsyncReadOnlyCollection<T>.this[Int32 index] => GetAt(index);

        public void Clear()
        {
            DoBlockingTransaction(_ => ClearImpl());
        }

        public void CopyTo(Array array, Int32 index)
        {
            ((ICollection) _backingCollection).CopyTo(array, index);
        }

        public Int32 Count
        {
            get => _count;
            private set => SetValue(ref _count, value);
        }

        public Object SyncRoot => ((ICollection) _backingCollection).SyncRoot;

        public Boolean IsSynchronized => ((ICollection) _backingCollection).IsSynchronized;


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Int32 Add(Object value)
        {
            _ui.Invoke(() => { AddImpl((T) value); });

            return _count;
        }

        public Boolean Contains(Object value)
        {
            return ((IList) _backingCollection).Contains(value);
        }


        public Int32 IndexOf(Object value)
        {
            return ((IList) _backingCollection).IndexOf(value);
        }

        public void Insert(Int32 index, Object value)
        {
            if (!(value is T ok))
                return;
            _ui.Invoke(() => InsertImpl(index, ok));
        }

        public void Remove(Object value)
        {
            _ui.Invoke(() =>
            {
                if (!(value is T ok) || !_hashCheck.Remove(ok))
                    return;

                RemoveImpl(ok);
            });
        }

        public void RemoveAt(Int32 index)
        {
            _ui.Invoke(() => { RemoveAtImpl(index); });
        }


        public Object this[Int32 index]
        {
            get => _ui.Invoke(() => ((IList) _backingCollection)[index]);
            set => _ui.Invoke(() => ((IList) _backingCollection)[index] = value);
        }

        public Boolean IsReadOnly => ((IList) _backingCollection).IsReadOnly;

        public Boolean IsFixedSize => ((IList) _backingCollection).IsFixedSize;

        T IReadOnlyList<T>.this[Int32 index] => GetAt(index);

        public T[] ToArray()
        {
            return _ui.Invoke(() => _backingCollection.ToArray());
        }

        private Boolean AddImpl(T item)
        {
            if (!_hashCheck.Add(item))
                return false;
            _count++;
            _backingCollection.Add(item);

            return true;
        }

        private void BuildTransactionSummary(ref NotifyCollectionChangedEventArgs? args)
        {
            if (CollectionChanged == null)
            {
                _addedInTransaction.Clear();
                _removedInTransaction.Clear();

                return;
            }

            var added = new List<T>();
            while (_addedInTransaction.TryDequeue(out var a))
                added.Add(a);

            var removed = new List<T>();
            while (_removedInTransaction.TryDequeue(out var r))
                removed.Add(r);


            if (added.Count > 0 && removed.Count == 0)
                args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, added);
            else if (added.Count == 0 && removed.Count > 0)
                args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed);
            else if ((added.Count > 0 || removed.Count > 0) && !added.SequenceEqual(removed))
                args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, added, removed);
        }

        private void ClearImpl()
        {
            _hashCheck.Clear();
            _count = 0;
            _backingCollection.Clear();
        }

        public void DoBlockingTransaction(Action<IAsyncObservableCollection<T>> action)
        {
            var args = _ui.Invoke(() => DoTransactionImpl(() => action(this)));
            if (args == null)
                return;

            NotifyCollectionChanged(args);
        }

        public async Task<Boolean> DoTransaction<TInput>(IEnumerable<TInput> datas,
                                                         Func<IAsyncObservableCollection<T>, TInput, Boolean> action)
        {
            var ds = datas.ToArray();

            void LetsRun()
            {
                foreach (var d in ds!)
                    action(this, d);
            }

            return await DoTransactionCore(LetsRun);
        }

        private async Task<Boolean> DoTransactionCore(Action action)
        {
            var args = await _ui.InvokeAsync(action, DoTransactionImpl);
            if (args == null)
                return false;

            await NotifyCollectionChangedAsync(args).ConfigureAwait(false);

            return true;
        }


        private NotifyCollectionChangedEventArgs? DoTransactionImpl(Action action)
        {
            NotifyCollectionChangedEventArgs? args = null;

            SuspendUp();

            try
            {
                action();
            }
            finally
            {
                if (SuspendDown() == 0)
                    BuildTransactionSummary(ref args);
            }

            return args;
        }

        public async Task<T> FirstOrDefaultAsync<TDerived>(Func<TDerived, Boolean> predicate)
            where TDerived : class, T
        {
            var got = await _ui.InvokeAsync(() =>
            {
                foreach (var b in _backingCollection.OfType<TDerived>())
                    if (predicate(b))
                        return b;

                return default!;
            });
            return got!;
        }


        private T GetAt(Int32 index)
        {
            return _ui.Invoke(() => _backingCollection[index]);
        }

        public IEnumerator<T> GetEnumerator()
        {
            var arr = _ui.Invoke(() => _backingCollection.ToArray());
            foreach (var r in arr)
                yield return r;
        }

        private Boolean GetIsSuspended()
        {
            var canI = 0;
            Interlocked.Exchange(ref canI, __suspendCounter);
            return canI > 0;
        }

        // ReSharper disable once UnusedMember.Global
        public Int32 IndexOf(T item)
        {
            return _ui.Invoke(() => _backingCollection.IndexOf(item));
        }

        private void InsertImpl(Int32 index, T value)
        {
            if (!_hashCheck.Add(value))
                return;

            _count++;

            _backingCollection.Insert(index, value);
        }

        private void NotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (!TryGetChangeHandler(out var change))
                return;

            _ui.Invoke(() =>
            {
                if (args != null)
                    change.Invoke(this, args);

                RaisePropertyChanged(nameof(Count));
                RaisePropertyChanged(IndexerName);
            });
        }

        private async Task NotifyCollectionChangedAsync(NotifyCollectionChangedEventArgs args)
        {
            if (!TryGetChangeHandler(out var change))
                return;

            await _ui.InvokeAsync(() =>
            {
                if (args != null)
                    change.Invoke(this, args);

                RaisePropertyChanged(nameof(Count));
                RaisePropertyChanged(IndexerName);
            }).ConfigureAwait(false);
        }

        private void OnBackingCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
        {
            if (GetIsSuspended())
            {
                if (e.NewItems != null)
                    foreach (var item in e.NewItems.OfType<T>())
                        _addedInTransaction.Enqueue(item);

                if (e.OldItems != null)
                    foreach (var item in e.OldItems.OfType<T>())
                        _removedInTransaction.Enqueue(item);

                return;
            }

            CollectionChanged?.Invoke(this, e);
        }

        private void OnBackingCollectionPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        protected async Task OnCollectionReset()
        {
            if (!TryGetChangeHandler(out var changed))
                return;

            await _ui.InvokeAsync(() =>
            {
                changed.Invoke(this, new NotifyCollectionChangedEventArgs
                    (NotifyCollectionChangedAction.Reset));
            }).ConfigureAwait(false);
        }

        private void RemoveAtImpl(Int32 index)
        {
            var item = _backingCollection[index];
            _hashCheck.Remove(item);

            _count--;

            _backingCollection.RemoveAt(index);
        }

        private Boolean RemoveImpl(T item)
        {
            if (!_hashCheck.Remove(item))
                return false;
            _count--;
            _backingCollection.Remove(item);

            return true;
        }

        private static IEnumerable<T> SelectFromMany(IEnumerable<T>[] many)
        {
            foreach (var m in many)
            foreach (var i in m)
                yield return i;
        }

        private Int32 SuspendDown()
        {
            return Interlocked.Decrement(ref __suspendCounter);
        }

        private void SuspendUp()
        {
            Interlocked.Increment(ref __suspendCounter);
        }

        public override String ToString()
        {
            return typeof(T).Name + "Items: " + _count + " Suspensions: " +
                   __suspendCounter + " " + GetHashCode();
        }

        private Boolean TryGetChangeHandler(out NotifyCollectionChangedEventHandler changed)
        {
            changed = CollectionChanged!;
            return changed != null;
        }

        private const String IndexerName = "Item[]";

        private readonly ConcurrentQueue<T> _addedInTransaction;

        protected readonly ObservableCollection<T> _backingCollection;
        private readonly HashSet<T> _hashCheck;
        private readonly ConcurrentQueue<T> _removedInTransaction;
        private readonly ISingleThreadedInvoker _ui;
        private Int32 __suspendCounter;


        private Int32 _count;
    }
}


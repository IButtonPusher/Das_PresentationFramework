﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Das.ViewModels.Collections;
using Das.Views.Mvvm;

namespace Das.ViewModels
{
    public class AsyncObservableCollection2<T> : AsyncObservableCollectionBase<T>,
                                                 IList,
                                                 IAsyncReadOnlyCollection<T>,
                                                 IAsyncObservableCollection<T>,
                                                 IReadOnlyList<T>
        where T : IEquatable<T>
    {
        public AsyncObservableCollection2()
            : this(Enumerable.Empty<T>())
        {
        }

        public AsyncObservableCollection2(IEnumerable<T> items)
        : this(new HashSet<T>(items))
        
        {
        }

        private AsyncObservableCollection2(HashSet<T> hashCheck)
        : this(hashCheck, new List<T>(hashCheck))
        {

        }

        private AsyncObservableCollection2(HashSet<T> hashCheck,
                                           List<T> backingCollection)
        : base(backingCollection, hashCheck)
        {
            _lock = new ReaderWriterAsync();
            //_hashCheck = hashCheck;
            //_backingCollection = backingCollection;
            
            //Count = _hashCheck.Count;
            //_addedInTransaction = new ConcurrentQueue<T>();
            //_removedInTransaction = new ConcurrentQueue<T>();
            
        }

        public AsyncObservableCollection2(params IEnumerable<T>[] itemses)
            : this(SelectFromMany(itemses))
        {
        }

        //public override void Dispose()
        //{
        //    base.Dispose();
        //    //CollectionChanged = null;
        //    //_backingCollection.CollectionChanged -= OnBackingCollectionChanged;

        //    //foreach (var bc in _backingCollection.OfType<IDisposable>())
        //    //    bc.Dispose();

        //    //INotifyPropertyChanged exp = _backingCollection;
        //    //exp.PropertyChanged -= OnBackingCollectionPropertyChanged;

        //    //_backingCollection.Clear();
        // //   Count = 0;
        //}

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
            return _lock.Write(item, RemoveImpl);
            //return _ui.Invoke(() => RemoveImpl(item));
        }

        T IAsyncObservableCollection<T>.this[Int32 index]
            => _lock.Read(() => _backingCollection[index]);

        T INotifyingCollection<T>.this[Int32 index] 
            => _lock.Read(() => _backingCollection[index]);

        public async Task<Boolean> AddAsync(T item)
        {
            var res = await _lock.WriteAsync(item, AddImpl).ConfigureAwait(false);

            //if (res && CollectionChanged is {})
            //{
            //    var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
            //        new T[] {item});
            //    await NotifyCollectionChangedAsync(args);
            //}

            return res;
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
            await _lock.WriteAsync(this, items, (t, i) => t.SynchronizeImpl(i));

            //await DoTransaction(k =>
            //{
            //    var itemSearch = new HashSet<T>(items);
            //    var remove = _backingCollection.Where(b => !itemSearch.Contains(b)).ToArray();
            //    for (var c = 0; c < remove.Length; c++)
            //    {
            //        _count--;
            //        _backingCollection.Remove(remove[c]);
            //        _hashCheck.Remove(remove[c]);
            //    }

            //    var add = itemSearch.Where(b => !_hashCheck.Contains(b)).ToArray();
            //    for (var c = 0; c < add.Length; c++)
            //    {
            //        _count++;
            //        _backingCollection.Add(add[c]);
            //        _hashCheck.Add(add[c]);
            //    }
            //});

            //OnCollectionReset();

            //await OnCollectionReset().ConfigureAwait(false);
        }

        public async Task Synchonize<TOther>(NotifyCollectionChangedEventArgs args,
                                             Func<T, TOther, Boolean> equate,
                                             Func<TOther, T> forNew)
        {
            await _lock.WriteTaskAsync(this, args, equate, forNew, async (t, a, e, f) =>
            {
                await t.SynchronizeImpl(a, e, f);
            });

            //await DoAsyncTransaction(async k =>
            //{
            //    if (args.OldItems is { } goneNow)
            //    {
            //        var going = goneNow.OfType<TOther>().ToArray();

            //        var rip = await SelectAsync(u => going.Any(g => equate(u, g)));
            //        await TryRemove(rip);
            //    }

            //    if (args.NewItems is IList someList)
            //    {
            //        var addMany = new List<T>();

            //        foreach (var added in someList.OfType<TOther>())
            //        {
            //            var letsAdd = forNew(added);
            //            addMany.Add(letsAdd);
            //        }

            //        await AddAsync(addMany);
            //    }
            //}).ConfigureAwait(false);
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
            await _lock.WriteAsync(this, async t => await action(t));

            //await NotifyCollectionChangedAsync(args).ConfigureAwait(false);
        }

        public async Task DoTransaction(Action<IAsyncObservableCollection<T>> action)
        {
            await _lock.WriteAsync(this, action, (t, a) => a(t)).ConfigureAwait(false);
        }

        public async Task DoTransaction<TInput>(IEnumerable<TInput> datas,
                                                Action<IAsyncObservableCollection<T>, TInput> action)
        {
            await _lock.WriteAsync(this, action, datas, (t, a, d) =>
            {
                foreach (var item in d)
                {
                    a(t, item);
                }
            });

            //var ds = datas.ToArray();

            //void LetsRun()
            //{
            //    foreach (var d in ds!)
            //        action(this, d);
            //}

            //await DoTransactionCore(LetsRun).ConfigureAwait(false);
        }

        //public async Task DoTransaction<TInput>(IAsyncEnumerable<TInput> datas,
        //                                        Action<IAsyncObservableCollection<T>, TInput> action)
        //{
        //    var ds = await datas.ToArrayAsync();

        //    void LetsRun()
        //    {
        //        foreach (var d in ds!)
        //            action(this, d);
        //    }

        //    await DoTransactionCore(LetsRun).ConfigureAwait(false);
        //}


        public async Task AddRangeAsync(IEnumerable<T> items)
        {

            await _lock.WriteAsync(this, items, (t, ioi) => t.AddRangeImpl(ioi));
            //await DoTransaction(items, (_, item) => { AddImpl(item); }).ConfigureAwait(false);

            //var newItems = await _lock.WriteAsync(this, items, (t, adding) =>
            //{
            //    var added = new List<T>();

            //    foreach (var item in adding)
            //    {
            //        if (t.AddImpl(item))
            //            added.Add(item);
            //    }

            //    return added;

            //}).ConfigureAwait(false);

            //if (newItems.Count == 0)
            //    return;

            //var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems);
            //await NotifyCollectionChangedAsync(args);

        }

        public async Task AddRangeAsync(IAsyncEnumerable<T> items)
        {
            await _lock.WriteAsync(this, items, async (t, ioi) => await t.AddRangeImpl(ioi));

            //var newItems = await _lock.WriteTaskAsync(this, items, async (t, adding) =>
            //{
            //    var added = new List<T>();

            //    await foreach (var item in adding)
            //    {
            //        if (t.AddImpl(item))
            //            added.Add(item);
            //    }

            //    return added;

            //}).ConfigureAwait(false);

            //if (newItems.Count == 0)
            //    return;

            //var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems);
            //await NotifyCollectionChangedAsync(args);

            //await DoTransaction(items, (_, item) => { AddImpl(item); }).ConfigureAwait(false);
        }

        public async Task<Boolean> ContainsAsync(T item)
        {
            return await  _lock.ReadAsync(item, _backingCollection, (i,c)
                => c.Contains(i));
        }

        public async Task<Int32> IndexOfAsync(T item)
        {
            return await _lock.ReadAsync(item, _backingCollection, 
                (i,c) => c.IndexOf(i));
        }

        public async Task<Boolean> AddOrUpdateAsync(T item)
        {
            if (await AddAsync(item))
                return true;

            await _lock.WriteAsync(item, i =>
            {
                UpdateImpl(i);
                //_hashCheck.Remove(i);
                //_hashCheck.Add(i);
                //var updating = _backingCollection.FirstOrDefault(b => b.Equals(i));

                //_backingCollection.Remove(updating);
                //_backingCollection.Add(i);
                return false;
            }).ConfigureAwait(false);

            return false;
        }

        public async Task<Boolean> TryRemoveAsync(T item)
        {
            return await _lock.WriteAsync(this, item, (t,i) => t.RemoveImpl(i));

            //var dude = true;

            //await DoTransaction(k =>
            //{
            //    if (!_hashCheck.Remove(item))
            //    {
            //        dude = false;
            //        return;
            //    }

            //    _count--;
            //    _backingCollection.Remove(item);
            //    _removedInTransaction.Enqueue(item);

            //}).ConfigureAwait(false);

            //return dude;
            //return await _lock.WriteAsync(item, RemoveImpl);
        }


        public async Task TryRemove(IEnumerable<T> items)
        {
            await _lock.WriteAsync(this, items, (t,ioi) => t.RemoveImpl(ioi));

            //var arr = new List<T>(items);

            //await DoTransaction(arr, (a, b) => { RemoveImpl(b); }).ConfigureAwait(false);
        }


        public async Task<Boolean> TryRemove(ICollection<T> items,
                                             IProgress<T> progress)
        {
            var bye = await _lock.WriteAsync(this, items, (t, ioi) =>
            {
                var removed = t.RemoveImpl(ioi);
                return removed;
                //if (RemoveImpl(b))
                //    progress.Report(b);
                //else return false;

                //return true;
            });

            var dothSatisfied = false;

            foreach (var byeBye in bye)
            {
                dothSatisfied = true;
                progress.Report(byeBye);
            }

            return dothSatisfied;
        }


        public async Task Remove(IEnumerable<T> items)
        {
            await _lock.WriteAsync(this, items, (t,ioi) => t.RemoveImpl(ioi));

            //await DoTransaction(k =>
            //{
            //    foreach (var item in items)
            //    {
            //        if (!_hashCheck.Remove(item))
            //            continue;

            //        _count--;
            //        _backingCollection.Remove(item);

            //        _removedInTransaction.Enqueue(item);
            //    }
            //}).ConfigureAwait(false);
        }

        public async Task ClearAsync()
        {
            await _lock.WriteAsync(this, (t) =>
            {
                t.ClearImpl();
                return -0;
            });

            //if (Count == 0)
            //    return;

            //await DoTransaction(_ => ClearImpl()).ConfigureAwait(false);
            //await OnCollectionReset().ConfigureAwait(false);
        }

        public override async Task<IReadOnlyList<T>> SelectAsync(Func<T, Boolean> predicate)
        {
            var got = await _lock.ReadAsync(_backingCollection, predicate, (b,p) =>
            {
                var res = GetNewReadOnlyList();

                foreach (var item in b)
                    if (p(item))
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

        public async Task<IReadOnlyList<T>> SelectAsync<TArg>(Func<T, TArg, Boolean> predicate, 
                                                              TArg data)
        {
            var got = await _lock.ReadAsync(this, predicate, data, (t,p,d) =>
            {
                var res = GetNewReadOnlyList();

                foreach (var item in t._backingCollection)
                    if (p(item, d))
                        res.Add(item);

                return res;
            });
            return got;
        }

        public async Task<TCollection> SelectAsync<TCollection, TOut>(Func<T, Boolean> predicate,
                                                                      Func<T, TOut> selecting)
            where TCollection : ICollection<TOut>, new()
        {
            return await _lock.ReadAsync(this, predicate, selecting, (t,p,s) =>
            {
                var items = t._backingCollection.Where(p);
                var res = new TCollection();
                foreach (var i in items)
                {
                    var shallAdd = s(i);
                    res.Add(shallAdd);
                }

                return res;
            });
        }

        public async Task<T> FirstOrDefaultAsync()
        {
            if (Count == 0)
                return default!;
            var got = await _lock.ReadAsync(_backingCollection, b =>
            {
                if (b.Count == 0)
                    return default!;

                return b[0];
            });
            return got;
        }

        public async Task<T> FirstOrDefaultAsync(Func<T, Boolean> predicate)
        {
            var got = await _lock.ReadAsync(_backingCollection, predicate, (b,p) =>
            {
                foreach (var i in b)
                    if (p(i))
                        return i;

                return default!;
            });
            return got;
        }

        public async Task<Boolean> AnyAsync(Func<T, Boolean> predicate)
        {
            var got = await _lock.ReadAsync(_backingCollection, predicate,
                (b,p) => b.Any(p));
            return got;
        }

        public async Task<Boolean> AllAsync(Func<T, Boolean> predicate)
        {
            var got = await _lock.ReadAsync(_backingCollection, predicate, (b,p) => b.All(p));
            return got;
        }


        public async Task<IReadOnlyList<T>> GetEnumerableAsync()
        {
            var got = await _lock.ReadAsync(_backingCollection, GetNewReadOnlyList);
            return got;
        }


        public void Add(T item)
        {
            _lock.Write(this, item, (t,i) => { t.AddImpl(i); });
        }

        public Boolean Contains(T item)
        {
            return _lock.Read(_backingCollection, item, (b,i) => b.Contains(i));
        }

        public void CopyTo(T[] array, Int32 arrayIndex)
        {
            _lock.Write(_backingCollection, array, arrayIndex,
                (b, a, i) => { b.CopyTo(a, i); });
        }


        public async Task RemoveAtAsync(Int32 index)
        {
            await _lock.WriteAsync<AsyncObservableCollection2<T>, Int32>(this, index, (t, i) => 
                 t.RemoveAtImpl(i)).ConfigureAwait(false);
        }

        public async Task InsertAsync(Int32 index, T item)
        {
            await _lock.WriteAsync(index, item, InsertImpl).ConfigureAwait(false);
        }


        //public event NotifyCollectionChangedEventHandler? CollectionChanged;

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        T IAsyncReadOnlyCollection<T>.this[Int32 index] => GetAt(index);

        public void Clear()
        {
            _lock.Write(ClearImpl);
            //DoBlockingTransaction(_ => ClearImpl());
        }

        public void CopyTo(Array array, Int32 index)
        {
            ((ICollection) _backingCollection).CopyTo(array, index);
        }

        //public Int32 Count
        //{
        //    get => _count;
        //    private set => SetValue(ref _count, value);
        //}

        public Object SyncRoot => ((ICollection) _backingCollection).SyncRoot;

        public Boolean IsSynchronized => ((ICollection) _backingCollection).IsSynchronized;


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Int32 Add(Object value)
        {
            _lock.Write(() => { AddImpl((T) value); });

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
            _lock.Write(() => InsertImpl(index, ok));
        }

        public void Remove(Object value)
        {
            _lock.Write(() =>
            {
                if (!(value is T ok))
                    return;

                RemoveImpl(ok);
            });
        }

        public void RemoveAt(Int32 index)
        {
            _lock.Write(() => { RemoveAtImpl(index); });
        }


        public Object this[Int32 index]
        {
            get => _lock.Read(() => ((IList) _backingCollection)[index]);
            set => _lock.Write(() => ((IList) _backingCollection)[index] = value);
        }

        public Boolean IsReadOnly => ((IList) _backingCollection).IsReadOnly;

        public Boolean IsFixedSize => ((IList) _backingCollection).IsFixedSize;

        T IReadOnlyList<T>.this[Int32 index] => GetAt(index);

        public T[] ToArray()
        {
            return _lock.Read(() => _backingCollection.ToArray());
        }

        //private Boolean AddImpl(T item)
        //{
        //    if (!_hashCheck.Add(item))
        //        return false;
        //    _count++;
        //    _backingCollection.Add(item);

        //    return true;
        //}

        //private void BuildTransactionSummary(ref NotifyCollectionChangedEventArgs? args)
        //{
        //    if (CollectionChanged == null)
        //    {
        //        _addedInTransaction.Clear();
        //        _removedInTransaction.Clear();

        //        return;
        //    }

        //    var added = new List<T>();
        //    while (_addedInTransaction.TryDequeue(out var a))
        //        added.Add(a);

        //    var removed = new List<T>();
        //    while (_removedInTransaction.TryDequeue(out var r))
        //        removed.Add(r);


        //    if (added.Count > 0 && removed.Count == 0)
        //        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, added);
        //    else if (added.Count == 0 && removed.Count > 0)
        //        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed);
        //    else if ((added.Count > 0 || removed.Count > 0) && !added.SequenceEqual(removed))
        //        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, added, removed);
        //}

        //private void ClearImpl()
        //{
        //    _hashCheck.Clear();
        //    _count = 0;
        //    _backingCollection.Clear();
        //}

        //public void DoBlockingTransaction(Action<IAsyncObservableCollection<T>> action)
        //{
        //    var args = _lock.Write(this, t => t.DoTransactionImpl(() => action(t)));
        //    if (args == null)
        //        return;

        //    NotifyCollectionChanged(args);
        //}

        //public async Task<Boolean> DoTransaction<TInput>(IEnumerable<TInput> datas,
        //                                                 Func<IAsyncObservableCollection<T>, TInput, Boolean> action)
        //{
        //    var ds = datas.ToArray();

        //    void LetsRun()
        //    {
        //        foreach (var d in ds!)
        //            action(this, d);
        //    }

        //    return await DoTransactionCore(LetsRun);
        //}

        //private async Task<Boolean> DoTransactionCore(Action action)
        //{
        //    var args = await _lock.WriteAsync(action, DoTransactionImpl);
        //    if (args == null)
        //        return false;

        //    await NotifyCollectionChangedAsync(args).ConfigureAwait(false);

        //    return true;
        //}


        //private NotifyCollectionChangedEventArgs? DoTransactionImpl(Action action)
        //{
        //    NotifyCollectionChangedEventArgs? args = null;

        //    SuspendUp();

        //    try
        //    {
        //        action();
        //    }
        //    finally
        //    {
        //        if (SuspendDown() == 0)
        //            BuildTransactionSummary(ref args);
        //    }

        //    return args;
        //}

        public async Task<T> FirstOrDefaultAsync<TDerived>(Func<TDerived, Boolean> predicate)
            where TDerived : class, T
        {
            var got = await _lock.ReadAsync(_backingCollection, predicate, (c,p) =>
            {
                foreach (var b in c.OfType<TDerived>())
                    if (p(b))
                        return b;

                return default!;
            });
            return got!;
        }


        private T GetAt(Int32 index)
        {
            return _lock.Read(_backingCollection, index, (b,i) => b[i]);
        }

        public IEnumerator<T> GetEnumerator()
        {
            var arr = _lock.ReadMany(_backingCollection, b => b.ToArray());
            foreach (var r in arr)
                yield return r;
        }

        //private Boolean GetIsSuspended()
        //{
        //    var canI = 0;
        //    Interlocked.Exchange(ref canI, __suspendCounter);
        //    return canI > 0;
        //}

        // ReSharper disable once UnusedMember.Global
        public Int32 IndexOf(T item)
        {
            return _lock.Read(_backingCollection, item, (b,i) => b.IndexOf(i));
        }

        //private void InsertImpl(Int32 index, T value)
        //{
        //    if (!_hashCheck.Add(value))
        //        return;

        //    _count++;

        //    _backingCollection.Insert(index, value);
        //}

        //private void NotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
        //{
        //    if (!TryGetChangeHandler(out var change))
        //        return;

        //    _lock.Write(() =>
        //    {
        //        if (args != null)
        //            change.Invoke(this, args);

        //        RaisePropertyChanged(nameof(Count), Count);
        //        RaisePropertyChanged(IndexerName);
        //    });
        //}

        //private async Task NotifyCollectionChangedAsync(NotifyCollectionChangedEventArgs args)
        //{
        //    if (!TryGetChangeHandler(out var change))
        //        return;

        //    await _lock.WriteAsync(args, change, (rdrr, c) =>
        //    {
        //        if (rdrr != null)
        //            c.Invoke(this, rdrr);

        //        RaisePropertyChanged(nameof(Count), Count);
        //        RaisePropertyChanged(IndexerName);
        //    }).ConfigureAwait(false);
        //}

        //private void OnBackingCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (GetIsSuspended())
        //    {
        //        if (e.NewItems != null)
        //            foreach (var item in e.NewItems.OfType<T>())
        //                _addedInTransaction.Enqueue(item);

        //        if (e.OldItems != null)
        //            foreach (var item in e.OldItems.OfType<T>())
        //                _removedInTransaction.Enqueue(item);

        //        return;
        //    }

        //    CollectionChanged?.Invoke(this, e);
        //}

        //private void OnBackingCollectionPropertyChanged(Object sender, PropertyChangedEventArgs e)
        //{
        //    RaisePropertyChanged(e.PropertyName);
        //}

        //protected async Task OnCollectionReset()
        //{
        //    if (!TryGetChangeHandler(out var changed))
        //        return;

        //    await _lock.WriteAsync(this, changed, (t,c) =>
        //    {
        //        c.Invoke(t, new NotifyCollectionChangedEventArgs
        //            (NotifyCollectionChangedAction.Reset));
        //    }).ConfigureAwait(false);
        //}

        //private void RemoveAtImpl(Int32 index)
        //{
        //    var item = _backingCollection[index];
        //    _hashCheck.Remove(item);

        //    _count--;

        //    _backingCollection.RemoveAt(index);
        //}

        //private Boolean RemoveImpl(T item)
        //{
        //    if (!_hashCheck.Remove(item))
        //        return false;
        //    _count--;
        //    _backingCollection.Remove(item);

        //    return true;
        //}

        private static IEnumerable<T> SelectFromMany(IEnumerable<T>[] many)
        {
            foreach (var m in many)
            foreach (var i in m)
                yield return i;
        }

        //private Int32 SuspendDown()
        //{
        //    return Interlocked.Decrement(ref __suspendCounter);
        //}

        //private void SuspendUp()
        //{
        //    Interlocked.Increment(ref __suspendCounter);
        //}

        public override String ToString()
        {
            return typeof(T).Name + "Items: " + _count;
        }

        //private Boolean TryGetChangeHandler(out NotifyCollectionChangedEventHandler changed)
        //{
        //    changed = CollectionChanged!;
        //    return changed != null;
        //}

        

        //private readonly ConcurrentQueue<T> _addedInTransaction;

        //protected readonly ObservableCollection<T> _backingCollection;
        //protected readonly List<T> _backingCollection;
        
        //private readonly ConcurrentQueue<T> _removedInTransaction;
        
        //private Int32 __suspendCounter;
        private readonly IReaderWriterAsync _lock;

    }
}

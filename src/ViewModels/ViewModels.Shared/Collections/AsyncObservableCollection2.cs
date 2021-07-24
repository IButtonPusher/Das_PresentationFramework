using System;
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
        }

        // ReSharper disable once UnusedMember.Global
        public AsyncObservableCollection2(params IEnumerable<T>[] itemses)
            : this(SelectFromMany(itemses))
        {
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
            await DoTransaction(_ =>
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
        }

        public async Task Synchonize<TOther>(NotifyCollectionChangedEventArgs args,
                                             Func<T, TOther, Boolean> equate,
                                             Func<TOther, T> forNew)
        {
            await _lock.WriteTaskAsync(this, args, equate, forNew, async (t, a, e, f) =>
            {
                await t.SynchronizeImpl(a, e, f);
            });
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
        }


        public async Task AddRangeAsync(IEnumerable<T> items)
        {
            await _lock.WriteAsync(this, items, (t, ioi) => t.AddRangeImpl(ioi));
        }

        public async Task AddRangeAsync(IAsyncEnumerable<T> items)
        {
            await _lock.WriteAsync(this, items, async (t, ioi) => await t.AddRangeImpl(ioi));
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
        }

        public async Task ClearAsync()
        {
            await _lock.WriteAsync(this, (t) =>
            {
                t.ClearImpl(true);
                return -0;
            });
        }

        public async Task<IReadOnlyList<T>> RemoveWhereAsync(Func<T, Boolean> predicate)
        {
            var got = await _lock.ReadAsync(_backingCollection, predicate, (b,p) =>
            {
                var res = GetNewReadOnlyList();

                foreach (var item in b)
                    if (p(item))
                        res.Add(item);

                foreach (var item in res)
                {
                    RemoveImpl(item);
                }

                return res;
            });
            return got;
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
            await _lock.WriteAsync(this, index, (t, i) => 
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
            _lock.Write(() => ClearImpl(false));
            OnCollectionReset();
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

        // ReSharper disable once UnusedMember.Global
        public T[] ToArray()
        {
            return _lock.Read(() => _backingCollection.ToArray());
        }

       

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

        // ReSharper disable once UnusedMember.Global
        public Int32 IndexOf(T item)
        {
            return _lock.Read(_backingCollection, item, (b,i) => b.IndexOf(i));
        }

       

        private static IEnumerable<T> SelectFromMany(IEnumerable<T>[] many)
        {
            foreach (var m in many)
            foreach (var i in m)
                yield return i;
        }

      
        public override String ToString()
        {
            return typeof(T).Name + "Items: " + _count;
        }

        private readonly IReaderWriterAsync _lock;

    }
}


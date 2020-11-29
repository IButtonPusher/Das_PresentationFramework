using Das.Views.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Das.ViewModels.Collections
{
    public abstract class AsyncObservableCollectionBase<T> : NotifyPropertyChangedBase
        where T : IEquatable<T>

    {

        protected readonly List<T> _backingCollection;
        private readonly HashSet<T> _hashCheck;
        protected Int32 _count;

        protected AsyncObservableCollectionBase(List<T> backingCollection,
                                                HashSet<T> hashCheck)
        {
            _backingCollection = backingCollection;
            _hashCheck = hashCheck;

            _count = _hashCheck.Count;
        }

        public override void Dispose()
        {
            base.Dispose();
            _count = 0;
            _hashCheck.Clear();
            _backingCollection.Clear();
            CollectionChanged = null;
        }

        public Int32 Count
        {
            get => _count;
            private set => SetValue(ref _count, value);
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        protected void UpdateImpl(T i)
        {
            _hashCheck.Remove(i);
            _hashCheck.Add(i);
            var removed = _backingCollection.FirstOrDefault(b => Equals(b, i))!;

            List<T> removedList;

            if (removed != null)
            {
                _backingCollection.Remove(removed);
                removedList = _singleRemovedList ??= new List<T>(1);
                removedList.Add(removed);
            }
            else removedList = _emptyList;

            _backingCollection.Add(i);

            var addedList = _singleAddedList ??= new List<T>(1);
            addedList.Add(i);

            NotifyCollectionChanged(addedList, removedList);
            addedList.Clear();
            removedList.Clear();
        }

        protected Boolean RemoveImpl(T item)
        {
            if (!RemoveImplNoNotify(item))
                return false;

            var singleRemoved = _singleRemovedList ??= new List<T>(1);
            singleRemoved.Add(item);

            NotifyCollectionChanged(_emptyList, singleRemoved);
            singleRemoved.Clear();
            return true;
        }

        protected IEnumerable<T> RemoveImpl(IEnumerable<T> items)
        {
            var removed = new List<T>();

            foreach (var item in items)
            {
                if (!RemoveImplNoNotify(item))
                    continue;

                removed.Add(item);
            }

            if (removed.Count > 0)
                NotifyCollectionChanged(_emptyList, removed);

            return removed;
        }

        private Boolean RemoveImplNoNotify(T item)
        {
            if (!_hashCheck.Remove(item))
            {
                return false;
            }

            _count--;
            _backingCollection.Remove(item);
            return true;
        }

        protected void RemoveAtImpl(Int32 index)
        {
            var item = _backingCollection[index];
            _hashCheck.Remove(item);

            _count--;

            _backingCollection.RemoveAt(index);

            var singleRemoved = _singleRemovedList ??= new List<T>(1);
            singleRemoved.Add(item);

            NotifyCollectionChanged(_emptyList, singleRemoved);

            singleRemoved.Clear();
        }

        protected async Task AddRangeImpl(IAsyncEnumerable<T> items)
        {
            var added = new List<T>();

            await foreach (var item in items)
            {
                if (AddImplNoNotify(item))
                    added.Add(item);
            }

            if (added.Count == 0)
                return;

            NotifyCollectionChanged(added, _emptyList);
        }

        protected void AddRangeImpl(IEnumerable<T> items)
        {
            var added = new List<T>();

            foreach (var item in items)
            {
                if (AddImplNoNotify(item))
                    added.Add(item);
            }

            if (added.Count == 0)
                return;

            NotifyCollectionChanged(added, _emptyList);
        }

        protected Boolean AddImpl(T item)
        {
            if (!AddImplNoNotify(item))
                return false;

            var single = _singleAddedList ??= new List<T>(1);
            single.Add(item);

            NotifyCollectionChanged(single, _emptyList);

            single.Clear();
            return true;
        }

        private Boolean AddImplNoNotify(T item)
        {
            if (!_hashCheck.Add(item))
                return false;
            _count++;
            _backingCollection.Add(item);

            return true;
        }

        protected void InsertImpl(Int32 index, 
                                  T value)
        {
            if (!_hashCheck.Add(value))
                return;

            _count++;

            _backingCollection.Insert(index, value);

            var singleAdded = _singleAddedList ??= new List<T>(1);
            singleAdded.Add(value);

            NotifyCollectionChanged(singleAdded, _emptyList);
            singleAdded.Clear();
        }

        protected void ClearImpl()
        {
            _hashCheck.Clear();
            _count = 0;
            _backingCollection.Clear();

           OnCollectionReset();
        }

        protected void SynchronizeImpl(IEnumerable<T> items)
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

            OnCollectionReset();
        }

        protected async Task SynchronizeImpl<TOther>(NotifyCollectionChangedEventArgs args,
                                              Func<T, TOther, Boolean> equate,
                                              Func<TOther, T> forNew)
        {
            var addedItems = new List<T>();
            var removedItems = new List<T>();

            if (args.OldItems is { } goneNow)
            {
                var going = goneNow.OfType<TOther>().ToArray();

                var rip = await SelectAsync(u => going.Any(g => equate(u, g)));
                foreach (var gg in rip)
                {
                    if (RemoveImplNoNotify(gg))
                        removedItems.Add(gg);
                }
            }

            if (args.NewItems is IList someList)
            {
                var addMany = new List<T>();

                foreach (var added in someList.OfType<TOther>())
                {
                    var letsAdd = forNew(added);
                    addMany.Add(letsAdd);
                }

                foreach (var hi in addMany)
                {
                    if (AddImplNoNotify(hi))
                        addedItems.Add(hi);
                }
            }

            NotifyCollectionChanged(addedItems, removedItems);
        }

        public abstract Task<IReadOnlyList<T>> SelectAsync(Func<T, Boolean> predicate);

        protected void OnCollectionReset()
        {
            if (!TryGetChangeHandler(out var changed))
                return;

            changed.Invoke(this, new NotifyCollectionChangedEventArgs
                (NotifyCollectionChangedAction.Reset));
        }

        protected static readonly List<T> _emptyList = new List<T>(0);

        [ThreadStatic]
        private static List<T>? _singleAddedList;

        [ThreadStatic]
        private static List<T>? _singleRemovedList;

        protected void NotifyCollectionChanged(List<T> added,
                                    List<T> removed)
        {
            if (TryGetChangeHandler(out var change))
            {

                NotifyCollectionChangedEventArgs? args = null;

                if (added.Count > 0 && removed.Count == 0)
                    args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, added);
                else if (added.Count == 0 && removed.Count > 0)
                    args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed);
                else if ((added.Count > 0 || removed.Count > 0) && !added.SequenceEqual(removed))
                    args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, added, removed);

                if (args != null)
                    change(this, args);
            }

            RaisePropertyChanged(nameof(Count), Count);
            RaisePropertyChanged(IndexerName);
        }

        private Boolean TryGetChangeHandler(out NotifyCollectionChangedEventHandler changed)
        {
            changed = CollectionChanged!;
            return changed != null;
        }

        private const String IndexerName = "Item[]";
    }
}

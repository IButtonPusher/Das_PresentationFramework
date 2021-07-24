using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Mvvm;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Views.Collections
{
    public class VisualCollection : IVisualCollection,
                                    INotifyingCollection<IVisualElement>,
                                    IList
    {
        public VisualCollection(IVisualCollection collection)
        : this(collection.GetFromEachChild(c => c))
        {

        }

        public VisualCollection(IEnumerable<IVisualElement> children)
        {
           _children = new List<IVisualElement>(children);
            _collectionHelper = new VisualCollectionHelper<IVisualElement>(_children, new Object());
        }

        public VisualCollection() : this(Enumerable.Empty<IVisualElement>())
        {

        }
        
        public void Add(IVisualElement element) => _collectionHelper.Add(element);
        
        public void AddRange(IEnumerable<IVisualElement> elements) => _collectionHelper.AddRange(elements);
        
        public Boolean Remove(IVisualElement element) => _collectionHelper.Remove(element);

        private readonly VisualCollectionHelper<IVisualElement> _collectionHelper;
        
        private readonly List<IVisualElement> _children;

        public IEnumerable<IVisualElement> GetAllChildren()
        {
            return _collectionHelper.GetChildrenOfType<IVisualElement>();
        }


        void ICollection.CopyTo(Array array, Int32 index)
        {
            ((ICollection) _collectionHelper).CopyTo(array, index);
        }

        public Int32 Count => _collectionHelper.Count;

        Boolean ICollection.IsSynchronized => ((ICollection) _collectionHelper).IsSynchronized;

        Object ICollection.SyncRoot => ((ICollection) _collectionHelper).SyncRoot;

        public Boolean Contains(IVisualElement element)
        {
            return _collectionHelper.Contains(element);
        }

        public Boolean IsTrueForAnyChild<TInput>(TInput input, 
                                                 Func<IVisualElement, TInput, Boolean> action)
        {
            return _collectionHelper.IsTrueForAnyChild(input, action);
        }

        public Boolean IsTrueForAnyChild(Func<IVisualElement, Boolean> action)
        {
            return _collectionHelper.IsTrueForAnyChild(action);
        }

        public IEnumerable<T> GetFromEachChild<T>(Func<IVisualElement, T> action)
        {
            return _collectionHelper.GetFromEachChild(action);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        public void RunOnEachChild(Action<IVisualElement> action)
        {
            _collectionHelper.RunOnEachChild(action);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        public void RunOnEachChild<TInput>(TInput input, 
                                           Action<TInput, IVisualElement> action)
        {
            _collectionHelper.RunOnEachChild(input, action);
        }

        public Task RunOnEachChildAsync<TInput>(TInput input, Func<TInput, IVisualElement, Task> action)
        {
            return _collectionHelper.RunOnEachChildAsync(input, action);
        }

        public void DistributeDataContext(Object? dataContext)
        {
            _collectionHelper.DistributeDataContext(dataContext);
        }

        public void InvalidateMeasure()
        {
            RunOnEachChild(child => child.InvalidateMeasure());
        }

        public void InvalidateArrange()
        {
            RunOnEachChild(child => child.InvalidateArrange());
        }

        public Boolean IsRequiresMeasure => _collectionHelper.IsRequiresMeasure;

        public Boolean IsRequiresArrange => _collectionHelper.IsRequiresArrange;

        public void AcceptChanges(ChangeType changeType)
        {
            
        }

        public void Clear(Boolean isDisposeVisuals) => _collectionHelper.Clear(isDisposeVisuals);
        
        public void Dispose() => _collectionHelper.Dispose();

        public IEnumerator<IVisualElement> GetEnumerator()
        {
            return _collectionHelper.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _collectionHelper).GetEnumerator();
        }

        Int32 IList.Add(Object value)
        {
            return ((IList) _collectionHelper).Add(value);
        }

        void IList.Clear()
        {
            ((IList) _collectionHelper).Clear();
        }

        Boolean IList.Contains(Object value)
        {
            return ((IList) _collectionHelper).Contains(value);
        }

        Int32 IList.IndexOf(Object value)
        {
            return ((IList) _collectionHelper).IndexOf(value);
        }

        void IList.Insert(Int32 index, Object value)
        {
            ((IList) _collectionHelper).Insert(index, value);
        }

        void IList.Remove(Object value)
        {
            ((IList) _collectionHelper).Remove(value);
        }

        public void RemoveAt(Int32 index)
        {
            ((IList) _collectionHelper).RemoveAt(index);
        }

        Boolean IList.IsFixedSize => ((IList) _collectionHelper).IsFixedSize;

        Boolean IList.IsReadOnly => ((IList) _collectionHelper).IsReadOnly;

        Object IList.this[Int32 index]
        {
            get => ((IList) _collectionHelper)[index];
            set => ((IList) _collectionHelper)[index] = value;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => _collectionHelper.CollectionChanged += value;
            remove => _collectionHelper.CollectionChanged -= value;
        }

        public override String ToString()
        {
            return "Visuals: " + _children.Count;
        }

        Object? INotifyingCollection.this[Int32 index] => ((INotifyingCollection) _collectionHelper)[index];

        public IVisualElement this[Int32 index] => _collectionHelper[index];
    }
}

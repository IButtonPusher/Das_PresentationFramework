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

#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace Das.Views.Collections
{


    public class VisualCollection<TVisual> : IVisualCollection<TVisual>,
                                             INotifyingCollection<TVisual>,
                                             IList,
                                             IVisualCollection
        where TVisual : IVisualElement
    {

        public VisualCollection(IVisualCollection<TVisual> collection)
            : this(collection.GetFromEachChild(c => c))
        {

        }

        public VisualCollection(IEnumerable<TVisual> children)
        {
            //_lockChildren = new Object();
            //_children = new List<TVisual>(children);
            _collectionHelper = new VisualCollectionHelper<TVisual>(
                new List<TVisual>(children), new Object());
        }
        
        //public IEnumerator<TVisual> GetEnumerator() => GetEnumeratorImpl();
        

        public VisualCollection() : this(Enumerable.Empty<TVisual>())
        {

        }

        //void ICollection.CopyTo(Array array,
        //                        Int32 index)
        //{
        //    lock (_lockChildren)
        //        ((ICollection) _children).CopyTo(array, index);
        //}

        void ICollection.CopyTo(Array array, Int32 index)
        {
            ((ICollection) _collectionHelper).CopyTo(array, index);
        }

        public Int32 Count => _collectionHelper.Count;

        Boolean ICollection.IsSynchronized => ((ICollection) _collectionHelper).IsSynchronized;

        Object ICollection.SyncRoot => ((ICollection) _collectionHelper).SyncRoot;


        Boolean IVisualCollection.Contains(IVisualElement element)
        {
            return _collectionHelper.Contains(element);
            
            //if (!(element is TVisual valid))
            //    return false;

            //return Contains(valid);
        }
        
        
        public IEnumerable<TVisual> GetAllChildren()
        {
            return _collectionHelper.GetChildrenOfType<TVisual>();
        }

        IEnumerable<IVisualElement> IVisualCollection.GetAllChildren()
        {
            return _collectionHelper.GetChildrenOfType<IVisualElement>();
        }

        Boolean IVisualCollection.IsTrueForAnyChild<TInput>(TInput input, 
                                                            Func<IVisualElement, TInput, Boolean> action)
        {
            return _collectionHelper.IsTrueForAnyChild(input, action);
            
            //lock (_lockChildren)
            //{
            //    foreach (var visual in _children)
            //    {
            //        if (action(visual, input))
            //            return true;
            //    }

            //    return false;
            //}
        }

        Boolean IVisualCollection.IsTrueForAnyChild(Func<IVisualElement, Boolean> action)
        {
            return _collectionHelper.IsTrueForAnyChild(action);

            //lock (_lockChildren)
            //{
            //    foreach (var visual in _children)
            //    {
            //        if (action(visual))
            //            return true;
            //    }

            //    return false;
            //}
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        IEnumerable<T> IVisualCollection.GetFromEachChild<T>(Func<IVisualElement, T> action)
        {
            return _collectionHelper.GetFromEachChild(action);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        void IVisualCollection.RunOnEachChild(Action<IVisualElement> action)
        {
            _collectionHelper.RunOnEachChild(action);
            //lock (_lockChildren)
            //{
            //    foreach (var visual in _children)
            //    {
            //        action(visual);
            //    }
            //}
        }

        void IVisualCollection.RunOnEachChild<TInput>(TInput input,
                                                                      Action<TInput, IVisualElement> action)
        {
            _collectionHelper.RunOnEachChild(input, action);
        }

        Task IVisualCollection.RunOnEachChildAsync<TInput>(TInput input, 
                                                                           Func<TInput, IVisualElement, Task> action)
        {
            return _collectionHelper.RunOnEachChildAsync(input, action);
        }


        public void Add(TVisual element) => _collectionHelper.Add(element);
       
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

        public void RemoveAt(Int32 index) => _collectionHelper.RemoveAt(index);

        Boolean IList.IsFixedSize => ((IList) _collectionHelper).IsFixedSize;

        Boolean IList.IsReadOnly => ((IList) _collectionHelper).IsReadOnly;

        Object IList.this[Int32 index]
        {
            get => ((IList) _collectionHelper)[index];
            set => ((IList) _collectionHelper)[index] = value;
        }
        //{
        //    lock (_lockChildren)
        //        _children.RemoveAt(index);
        //}

        public Boolean Remove(TVisual element) => _collectionHelper.Remove(element);
        //{
        //    lock (_lockChildren)
        //        return _children.Remove(element);
        //}


        public Boolean Contains(TVisual element)
        {
            return _collectionHelper.Contains(element);
            //lock (_lockChildren)
            //{
            //    foreach (var child in _children)
            //    {
            //        if (ReferenceEquals(child, element))
            //            return true;

            //        return child is IVisualContainer container &&
            //               container.Contains(element);
            //    }

            //    return false;
            //}
        }

        public void Clear(Boolean isDisposeVisuals) => _collectionHelper.Clear(isDisposeVisuals);
        //{
        //    lock (_lockChildren)
        //    {
        //        if (isDisposeVisuals)
        //        {
        //            for (var c = 0; c < _children.Count; c++)
        //                _children[c].Dispose();
        //        }

        //        _children.Clear(); //todo: more efficient
        //    }
        //}

        public void AddRange(IEnumerable<IVisualElement> elements) => _collectionHelper.AddRange(elements);
        
        public void AddRange(IEnumerable<TVisual> elements) => _collectionHelper.AddRange(elements);
        //{
        //    lock (_lockChildren)
        //    {
        //        _children.AddRange(elements);
        //    }
        //}

        public Boolean IsTrueForAnyChild<TInput>(TInput input,
                                                 Func<TVisual, TInput, Boolean> action)
        {
            return _collectionHelper.IsTrueForAnyChild(input, action);
            
            //lock (_lockChildren)
            //{
            //    foreach (var visual in _children)
            //    {
            //        if (action(visual, input))
            //            return true;
            //    }

            //    return false;
            //}
        }

        public Boolean IsTrueForAnyChild(Func<TVisual, Boolean> action)
        {
            return _collectionHelper.IsTrueForAnyChild(action);

            //lock (_lockChildren)
            //{
            //    foreach (var visual in _children)
            //    {
            //        if (action(visual))
            //            return true;
            //    }

            //    return false;
            //}
        }

        public TVisual this[Int32 index]
        {
            get => _collectionHelper[index];

        }
        
        
        [DebuggerStepThrough]
        [DebuggerHidden]
        public IEnumerable<T1> GetFromEachChild<T1>(Func<TVisual, T1> action)
        {
            return _collectionHelper.GetFromEachChild(action);
            //lock (_lockChildren)
            //{
            //    foreach (var visual in _children)
            //    {
            //        yield return action(visual);
            //    }
            //}
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        public void RunOnEachChild(Action<TVisual> action)
        {
            _collectionHelper.RunOnEachChild(action);
            //lock (_lockChildren)
            //{
            //    foreach (var visual in _children)
            //    {
            //        action(visual);
            //    }
            //}
        }

        
        [DebuggerStepThrough]
        [DebuggerHidden]
        public void RunOnEachChild<TInput>(TInput input,
                                           Action<TInput, TVisual> action)
        {
            _collectionHelper.RunOnEachChild(input, action);
            //lock (_lockChildren)
            //{
            //    foreach (var visual in _children)
            //    {
            //        action(input, visual);
            //    }
            //}
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        public Task RunOnEachChildAsync<TInput>(TInput input,
                                                      Func<TInput, TVisual, Task> action)
        {
            return _collectionHelper.RunOnEachChildAsync(input, action);
            
            //var running = new List<Task>();

            //lock (_lockChildren)
            //{
            //    foreach (var visual in _children)
            //    {
            //        running.Add(action(input, visual));
            //    }
            //}

            //await TaskEx.WhenAll(running);
        }

        //private readonly Object _lockChildren;
        //private readonly List<TVisual> _children;
        private readonly VisualCollectionHelper<TVisual> _collectionHelper;

        public void Dispose() => _collectionHelper.Dispose();
        //{
        //    Clear(true);
        //}

        public void InvalidateMeasure()
        {
            RunOnEachChild(child => child.InvalidateMeasure());
        }

        public void InvalidateArrange()
        {
            RunOnEachChild(child => child.InvalidateArrange());
        }

        // could call IsTrueForAnyChild here but want to save the MultiCastDelegate instantiation
        public Boolean IsRequiresMeasure => _collectionHelper.IsRequiresMeasure;
        //{
        //    get
        //    {
        //        lock (_lockChildren)
        //        {
        //            foreach (var visual in _children)
        //            {
        //                if (visual.IsRequiresMeasure)
        //                    return true;
        //            }

        //            return false;
        //        }
        //    }
        //}

        public Boolean IsRequiresArrange => _collectionHelper.IsRequiresArrange;
        //{
        //    get
        //    {
        //        lock (_lockChildren)
        //        {
        //            foreach (var visual in _children)
        //            {
        //                if (visual.IsRequiresArrange)
        //                    return true;
        //            }

        //            return false;
        //        }
        //    }
        //}

        public void AcceptChanges(ChangeType changeType)
        {
            //lock (_lockChildren)
            //{
            //    foreach (var visual in _children)
            //    {
            //        visual.AcceptChanges(changeType);
            //    }
            //}
        }

        public IEnumerator<TVisual> GetEnumerator()
        {
            return _collectionHelper.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => _collectionHelper.GetEnumerator();
        //{
        //    List<IVisualElement> res;

        //    lock (_lockChildren)
        //    {
        //        res = new List<IVisualElement>();
        //        foreach (var child in _children)
        //            res.Add(child);
        //    }

        //    return res.GetEnumerator();
        //}

        //IEnumerator<TVisual> IEnumerable<TVisual>.GetEnumerator() => GetEnumeratorImpl();

        //private IEnumerator<TVisual> GetEnumeratorImpl()
        //{
        //    List<TVisual> res;

        //    lock (_lockChildren)
        //        res = new List<TVisual>(_children);

        //    return res.GetEnumerator();
        //}

        //public IEnumerator<IVisualElement> GetEnumerator()
        //{
        //    List<IVisualElement> res;

        //    lock (_lockChildren)
        //    {
        //        res = new List<IVisualElement>();
        //        foreach (var child in _children)
        //            res.Add(child);
        //    }

        //    return res.GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorImpl();

        //Int32 IList.Add(Object value)
        //{
        //    if (!(value is TVisual visual))
        //        return -1;
        //    Add(visual);
        //    return Count - 1;
        //}

        //void IList.Clear()
        //{
        //    Clear(true);
        //}

        //Boolean IList.Contains(Object value)
        //{
        //    return value is TVisual visual && Contains(visual);
        //}

        //Int32 IList.IndexOf(Object value)
        //{
        //    if (!(value is TVisual visual))
        //        return -1;

        //    lock (_lockChildren)
        //        return _children.IndexOf(visual);
        //}

        //void IList.Insert(Int32 index,
        //                  Object value)
        //{
        //    if (!(value is TVisual visual))
        //        return;

        //    lock (_lockChildren)
        //        _children.Insert(index, visual);
        //}

        //void IList.Remove(Object value)
        //{
        //    if (!(value is TVisual visual))
        //        return;

        //    Remove(visual);
        //}

        //void IList.RemoveAt(Int32 index)
        //{
        //    lock (_lockChildren)
        //        _children.RemoveAt(index);
        //}

        //Boolean IList.IsFixedSize
        //{
        //    get
        //    {
        //        lock (_lockChildren)
        //            return ((IList) _children).IsFixedSize;
        //    }
        //}

        //Boolean IList.IsReadOnly
        //{
        //    get
        //    {
        //        lock (_lockChildren)
        //            return ((IList) _children).IsReadOnly;
        //    }
        //}

        //Object? IList.this[Int32 index]
        //{
        //    get => this[index];
        //    set => this[index] = value;
        //    //get
        //    //{

        //    //    lock (_lockChildren)
        //    //        return _children[index];

        //    //}
        //    //set
        //    //{
        //    //    if (!(value is TVisual visual))
        //    //        return;

        //    //    lock (_lockChildren)
        //    //        _children[index] = visual;
        //    //}
        //}

        //public Object? this[Int32 index]
        //{
        //    get
        //    {

        //        lock (_lockChildren)
        //            return _children[index];

        //    }
        //    set
        //    {
        //        if (!(value is TVisual visual))
        //            return;

        //        lock (_lockChildren)
        //            _children[index] = visual;
        //    }

        //}

        //public event NotifyCollectionChangedEventHandler? CollectionChanged;

        //Object? INotifyingCollection.this[Int32 index] => this[index];

        //TVisual INotifyingCollection<TVisual>.this[Int32 index] => (TVisual) this[index]!;

        //IVisualElement INotifyingCollection.this[Int32 index] => (IVisualElement)this[index]!;
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => _collectionHelper.CollectionChanged += value;
            remove => _collectionHelper.CollectionChanged -= value;
        }

        Object? INotifyingCollection.this[Int32 index] => ((INotifyingCollection) _collectionHelper)[index];
        
        public override String ToString()
        {
            return _collectionHelper.ToString();
        }
    }
}

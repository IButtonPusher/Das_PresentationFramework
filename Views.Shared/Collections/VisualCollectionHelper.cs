using Das.Views.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Mvvm;
using Das.Views.Panels;

#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace Das.Views.Collections
{
    public class VisualCollectionHelper<TVisual> : NotifyPropertyChangedBase, 
                                                   IList,
                                                   INotifyingCollection<TVisual> 
        where TVisual : IVisualElement
    {
        private readonly List<TVisual> _children;
        private readonly Object _lockChildren;

        public VisualCollectionHelper(List<TVisual> children,
            Object lockChildren)
        {
            _children = children;
            _lockChildren = lockChildren;
        }
        
        public void Add(TVisual element)
        {
            lock (_lockChildren)
            {
                _children.Add(element);
            }
            
            NotifyCollectionChanged(new List<TVisual> { element}, _emptyList);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            Clear(true);
        }

        public void Clear(Boolean isDisposeVisuals)
        {
            lock (_lockChildren)
            {
                if (isDisposeVisuals)
                {
                    for (var c = 0; c < _children.Count; c++)
                        _children[c].Dispose();
                }

                _children.Clear(); //todo: more efficient
            }
            
            OnCollectionReset();
        }
        
        protected void OnCollectionReset()
        {
            if (!TryGetChangeHandler(out var changed))
                return;

            changed.Invoke(this, new NotifyCollectionChangedEventArgs
                (NotifyCollectionChangedAction.Reset));
        }

        public void AddRange(IEnumerable<TVisual> elements)
        {
            var added = new List<TVisual>();
            
            lock (_lockChildren)
            {
                added.AddRange(elements);
                _children.AddRange(added);
            }
            
            NotifyCollectionChanged(added, _emptyList);
        }
        
        public void AddRange<TVisualAs>(IEnumerable<TVisualAs> elements)
            where TVisualAs :  IVisualElement
        {
            AddRange(elements.OfType<TVisual>());
            
            //lock (_lockChildren)
            //{
            //    foreach (var element in elements)
            //    {
            //        if (element is TVisual valid)
            //            _children.Add(valid);
            //    }
            //}
        }
        
        public void RemoveAt(Int32 index)
        {
            TVisual removed;
            
            lock (_lockChildren)
            {
                removed = _children[index];
                
                _children.RemoveAt(index);
            }

            NotifyCollectionChanged(_emptyList, new List<TVisual> {removed});
        }

        public Boolean Remove(TVisual element)
        {
            try
            {
                lock (_lockChildren)
                    return _children.Remove(element);
            }
            finally
            {
                NotifyCollectionChanged(_emptyList, new List<TVisual> {element});
            }
        }
        
        public Boolean IsTrueForAnyChild<TInput, TVisualAs>(TInput input,
                                                 Func<TVisualAs, TInput, Boolean> action)
        where TVisualAs :  IVisualElement
        {
            lock (_lockChildren)
            {
                foreach (var visual in _children)
                {
                    if (visual is TVisualAs valid &&
                        action(valid, input))
                        return true;
                }

                return false;
            }
        }
        
        public Int32 Count
        {
            get
            {
                lock (_lockChildren)
                    return _children.Count;
            }
        }

        private static readonly List<TVisual> _emptyList = new List<TVisual>();
        
        protected void NotifyCollectionChanged(List<TVisual> added,
                                               List<TVisual> removed)
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

        public Boolean IsTrueForAnyChild<TVisualAs>(Func<TVisualAs, Boolean> action)
            where TVisualAs : IVisualElement
        {
            lock (_lockChildren)
            {
                foreach (var visual in _children)
                {
                    if (visual is TVisualAs valid && action(valid))
                        return true;
                }

                return false;
            }
        }
        
        public Boolean Contains(IVisualElement element)
        {
            lock (_lockChildren)
            {
                foreach (var child in _children)
                {
                    if (ReferenceEquals(child, element))
                        return true;

                    return child is IVisualContainer container &&
                           container.Contains(element);
                }

                return false;
            }
        }
        
        [DebuggerStepThrough]
        [DebuggerHidden]
        public IEnumerable<T1> GetFromEachChild<T1, TVisualAs>(Func<TVisualAs, T1> action)
            where TVisualAs : IVisualElement
        {
            lock (_lockChildren)
            {
                foreach (var visual in _children)
                {
                    if (visual is TVisualAs valid)
                        yield return action(valid);
                }
            }
        }
        
        [DebuggerStepThrough]
        [DebuggerHidden]
        public void RunOnEachChild<TVisualAs>(Action<TVisualAs> action)
            where TVisualAs : IVisualElement
        {
            lock (_lockChildren)
            {
                foreach (var visual in _children)
                {
                    if (visual is TVisualAs valid)
                        action(valid);
                }
            }
        }
        
        [DebuggerStepThrough]
        [DebuggerHidden]
        public void RunOnEachChild<TInput, TVisualAs>(TInput input,
                                                      Action<TInput, TVisualAs> action)
            where TVisualAs : IVisualElement
        {
            lock (_lockChildren)
            {
                foreach (var visual in _children)
                {
                    if (visual is TVisualAs valid)
                        action(input, valid);
                }
            }
        }
        
        [DebuggerStepThrough]
        [DebuggerHidden]
        public Task RunOnEachChildAsync<TInput, TVisualAs>(TInput input,
                                                      Func<TInput, TVisualAs, Task> action)
            where TVisualAs : IVisualElement
        {
            var running = new List<Task>();

            lock (_lockChildren)
            {
                foreach (var visual in _children)
                {
                    if (visual is TVisualAs valid)
                        running.Add(action(input, valid));
                }
            }

            return TaskEx.WhenAll(running);
        }
        
        public Boolean IsRequiresMeasure
        {
            get
            {
                lock (_lockChildren)
                {
                    foreach (var visual in _children)
                    {
                        if (visual.IsRequiresMeasure)
                            return true;
                    }

                    return false;
                }
            }
        }
        
        public Boolean IsRequiresArrange
        {
            get
            {
                lock (_lockChildren)
                {
                    foreach (var visual in _children)
                    {
                        if (visual.IsRequiresArrange)
                            return true;
                    }

                    return false;
                }
            }
        }
        
         Int32 IList.Add(Object value)
        {
            if (!(value is TVisual visual))
                return -1;
            Add(visual);
            return Count - 1;
        }

        void IList.Clear()
        {
            Clear(true);
        }

        Boolean IList.Contains(Object value)
        {
            return value is TVisual visual && Contains(visual);
        }

        Int32 IList.IndexOf(Object value)
        {
            if (!(value is TVisual visual))
                return -1;

            lock (_lockChildren)
                return _children.IndexOf(visual);
        }

        void IList.Insert(Int32 index,
                          Object value)
        {
            if (!(value is TVisual visual))
                return;

            lock (_lockChildren)
                _children.Insert(index, visual);
        }

        void IList.Remove(Object value)
        {
            if (!(value is TVisual visual))
                return;

            Remove(visual);
        }

        void IList.RemoveAt(Int32 index)
        {
            lock (_lockChildren)
                _children.RemoveAt(index);
        }

        Boolean IList.IsFixedSize
        {
            get
            {
                lock (_lockChildren)
                    return ((IList) _children).IsFixedSize;
            }
        }

        Boolean IList.IsReadOnly
        {
            get
            {
                lock (_lockChildren)
                    return ((IList) _children).IsReadOnly;
            }
        }
        
        public TVisual this[Int32 index]
        {
            get
            {

                lock (_lockChildren)
                    return _children[index];

            }
            set
            {
                if (!(value is TVisual visual))
                    return;

                lock (_lockChildren)
                    _children[index] = visual;
            }

        }

        Object? IList.this[Int32 index]
        {
            get => this[index];
            set
            {
                if (value is TVisual vis)
                    this[index] = vis;
            } 
            //get
            //{

            //    lock (_lockChildren)
            //        return _children[index];

            //}
            //set
            //{
            //    if (!(value is TVisual visual))
            //        return;

            //    lock (_lockChildren)
            //        _children[index] = visual;
            //}
        }
        
        void ICollection.CopyTo(Array array,
                                Int32 index)
        {
            lock (_lockChildren)
                ((ICollection) _children).CopyTo(array, index);
        }
        
        Boolean ICollection.IsSynchronized
        {
            get
            {
                lock (_lockChildren)
                    return ((IList) _children).IsSynchronized;
            }
        }

        Object ICollection.SyncRoot
        {
            get
            {
                lock (_lockChildren)
                    return ((ICollection) _children).SyncRoot;
            }
        }
        
        //IEnumerator<TVisual> IEnumerable<TVisual>.GetEnumerator() => GetEnumeratorImpl();

        public IEnumerator<TVisual> GetEnumerator()
        {
            return GetEnumeratorImpl();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorImpl();

        public IEnumerable<TVisualAs> GetChildrenOfType<TVisualAs>()
        where TVisualAs : IVisualElement
        {
            List<TVisualAs> res;
            
            lock (_lockChildren)
            {
                res = new List<TVisualAs>();
                foreach (var child in _children)
                {
                    if (child is TVisualAs valid)
                        res.Add(valid);
                }
            }
            
            return res;
        }

        private IEnumerator<TVisual> GetEnumeratorImpl()
        {
            List<TVisual> res;

            lock (_lockChildren)
                res = new List<TVisual>(_children);

            return res.GetEnumerator();
        }

        public override String ToString()
        {
            lock (_lockChildren)
                return _children.ToString();
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        Object INotifyingCollection.this[Int32 index] => this[index];
        
        private const String IndexerName = "Item[]";
    }
}

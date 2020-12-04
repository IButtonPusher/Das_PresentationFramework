using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public class VisualCollection : IVisualCollection,
                                    IList
    {
        public VisualCollection()
        {
            _lockChildren = new Object();
            _children = new List<IVisualElement>();
        }

        void ICollection.CopyTo(Array array, 
                                Int32 index)
        {
            lock (_lockChildren)
                ((ICollection)_children).CopyTo(array, index);
        }

        public Int32 Count
        {
            get
            {
                lock (_lockChildren)
                    return _children.Count;
            }
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

        public void Add(IVisualElement element)
        {
            lock (_lockChildren)
            {
                _children.Add(element);
            }
        }

        public Boolean Remove(IVisualElement element)
        {
            lock (_lockChildren)
                return _children.Remove(element);
        }

        public Boolean Contains(IVisualElement element)
        {
            lock (_lockChildren)
            {
                foreach (var child in _children)
                {
                    if (child == element)
                        return true;

                    return child is IVisualContainer container &&
                           container.Contains(element);
                }

                return false;
            }
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
        }

        public void AddRange(IEnumerable<IVisualElement> elements)
        {
            lock (_lockChildren)
            {
                _children.AddRange(elements);
            }
        }

        public Boolean IsTrueForAnyChild<TInput>(TInput input,
                                                 Func<IVisualElement, TInput, Boolean> action)
        {
            lock (_lockChildren)
            {
                foreach (var visual in _children)
                {
                    if (action(visual, input))
                        return true;
                }

                return false;
            }
        }

        public Boolean IsTrueForAnyChild(Func<IVisualElement, Boolean> action)
        {
            lock (_lockChildren)
            {
                foreach (var visual in _children)
                {
                    if (action(visual))
                        return true;
                }

                return false;
            }
        }

        public IEnumerable<T1> GetFromEachChild<T1>(Func<IVisualElement, T1> action)
        {
            lock (_lockChildren)
            {
                foreach (var visual in _children)
                {
                    yield return action(visual);
                }
            }
        }

        public void RunOnEachChild(Action<IVisualElement> action)
        {
            lock (_lockChildren)
            {
                foreach (var visual in _children)
                {
                    action(visual);
                }
            }
        }

        public void RunOnEachChild<TInput>(TInput input, 
                                           Action<TInput, IVisualElement> action)
        {
            lock (_lockChildren)
            {
                foreach (var visual in _children)
                {
                    action(input, visual);
                }
            }
        }

        public async Task RunOnEachChildAsync<TInput>(TInput input, 
                                                Func<TInput, IVisualElement, Task> action)
        {
            var running = new List<Task>();

            lock (_lockChildren)
            {
                foreach (var visual in _children)
                {
                    running.Add(action(input, visual));
                }
            }

            await Task.WhenAll(running);
        }

        private readonly Object _lockChildren;
        private readonly List<IVisualElement> _children;

        public void Dispose()
        {
            Clear(true);
        }

        public void InvalidateMeasure()
        {
            RunOnEachChild(child => child.InvalidateMeasure());
        }

        public void InvalidateArrange()
        {
            RunOnEachChild(child => child.InvalidateArrange());
        }

        // could call IsTrueForAnyChild here but want to save the MultiCastDelegate instantiation
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            IVisualElement[] res;

            lock (_lockChildren)
                res = _children.ToArray();

            return res.GetEnumerator();
        }

        Int32 IList.Add(Object value)
        {
            if (!(value is IVisualElement visual))
                return -1;
            Add(visual);
            return Count-1;
        }

        void IList.Clear()
        {
            Clear(true);
        }

        Boolean IList.Contains(Object value)
        {
            return value is IVisualElement visual && Contains(visual);
        }

        Int32 IList.IndexOf(Object value)
        {
            if (!(value is IVisualElement visual))
                return -1;

            lock (_lockChildren)
                return _children.IndexOf(visual);
        }

        void IList.Insert(Int32 index, 
                          Object value)
        {
            if (!(value is IVisualElement visual))
                return;

            lock (_lockChildren)
                _children.Insert(index, visual);
        }

        void IList.Remove(Object value)
        {
            if (!(value is IVisualElement visual))
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

        Object IList.this[Int32 index]
        {
            get
            {

                lock (_lockChildren)
                    return _children[index];

            }
            set
            {
                if (!(value is IVisualElement visual))
                    return;

                lock (_lockChildren)
                    _children[index] = visual;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Views.Layout
{
    public class VisualLineage : IVisualLineage
    {
        public VisualLineage()
        {
            _visualStack = new Stack<IVisualElement>();
            _lock = new Object();
            _iterationStack = new Stack<IVisualElement>();
        }
        
        public IEnumerator<IVisualElement> GetEnumerator()
        {
            lock (_lock)
            {
                try
                {

                    while (_visualStack.Count > 0)
                    {
                        var current = _visualStack.Pop();
                        _iterationStack.Push(current);
                        yield return current;
                    }
                }
                finally
                {
                    while (_iterationStack.Count > 0)
                    {
                        _visualStack.Push(_iterationStack.Pop());
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void PushVisual(IVisualElement visual)
        {
           lock (_lock)
                _visualStack.Push(visual);
        }

        public IVisualElement PopVisual()
        {

            lock (_lock)
            {
                var res =_visualStack.Pop();
                return res;
            }
        }

        public void AssertPopVisual(IVisualElement visual)
        {
            var popped = PopVisual();
            if (!ReferenceEquals(popped, visual))
                throw new InvalidOperationException();
        }

        public IVisualElement? PeekVisual()
        {
            lock (_lock)
            {
                if (_visualStack.Count == 0)
                    return default;
                return _visualStack.Peek();
            }
        }

        public IVisualElement? GetNextSibling()
        {
            lock (_lock)
            {
                if (_visualStack.Count < 2)
                    return default;

                IVisualElement? res = default;

                var currentVisual = PopVisual();

                var parent = PeekVisual();
                if (parent is IVisualContainer container)
                {
                    for (var c = 0; c < container.Children.Count - 1; c++)
                    {
                        if (ReferenceEquals(currentVisual, container.Children[c]))
                        {
                            res = container.Children[c + 1];
                            break;
                        }
                    }
                }

                PushVisual(currentVisual);

                return res;
            }
        }

        public override String ToString()
        {
            lock (_lock)
                return "Visual stack - count: " + Count + " - " + String.Join(", ", this);
        }

        public Int32 Count
        {
            get
            {
                lock (_lock)
                    return _visualStack.Count;
            }
        }

        private readonly Stack<IVisualElement> _visualStack;
        private readonly Object _lock;
        private readonly Stack<IVisualElement> _iterationStack;
    }
}

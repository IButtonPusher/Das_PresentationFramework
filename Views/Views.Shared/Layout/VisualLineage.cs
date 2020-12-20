using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
                return _visualStack.Pop();
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
        
        private readonly Stack<IVisualElement> _visualStack;
        private readonly Object _lock;
        private readonly Stack<IVisualElement> _iterationStack;
    }
}

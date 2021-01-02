using System;
using System.Collections.Generic;

namespace Das.Views.Layout
{
    public class LayoutQueue : ILayoutQueue
    {
        public LayoutQueue()
        {
            _measureLock = new Object();
            _measureSearch = new HashSet<IVisualElement>();
            _measureQueue = new Queue<IVisualElement>();

            _arrangeLock = new Object();
            _arrangeSearch = new HashSet<IVisualElement>();
            _arrangeQueue = new Queue<IVisualElement>();
        }

        public Boolean HasVisualsNeedingLayout => HasVisualsNeedingArrange || HasVisualsNeedingMeasure;

        public void QueueVisualForMeasure(IVisualElement visual)
        {
            lock (_measureLock)
            {
                if (!_measureSearch.Add(visual))
                    return;

                _measureQueue.Enqueue(visual);
            }
        }

        public Boolean HasVisualsNeedingMeasure 
        {
            get
            {
                lock (_measureLock)
                    return _measureQueue.Count > 0;
            }
        }

        public Boolean TryDequeueVisualNeedingMeasure(out IVisualElement visual)
        {
            lock (_measureLock)
            {
                while (true)
                {
                    if (_measureQueue.Count == 0)
                    {
                        visual = default!;
                        return false;
                    }

                    visual = _measureQueue.Dequeue();
                    if (!_measureSearch.Remove(visual))
                        continue;

                    return true;
                }
            }
        }

        public void RemoveVisualFromMeasureQueue(IVisualElement visual)
        {
            lock (_measureLock)
                _measureSearch.Remove(visual);
        }

        public void QueueVisualForArrange(IVisualElement visual)
        {
            lock (_arrangeLock)
            {
                if (!_arrangeSearch.Add(visual))
                    return;

                _arrangeQueue.Enqueue(visual);
            }
        }

        public Boolean HasVisualsNeedingArrange
        {
            get
            {
                lock (_arrangeLock)
                    return _arrangeQueue.Count > 0;
            }
        }

        public Boolean TryDequeueVisualNeedingArrange(out IVisualElement visual)
        {
            lock (_arrangeLock)
            {
                while (true)
                {
                    if (_arrangeQueue.Count == 0)
                    {
                        visual = default!;
                        return false;
                    }

                    visual = _arrangeQueue.Dequeue();
                    if (!_arrangeSearch.Remove(visual))
                        continue;

                    return true;
                }
            }
        }

        public void RemoveVisualFromArrangeQueue(IVisualElement visual)
        {
            lock (_arrangeLock)
                _arrangeSearch.Remove(visual);
        }

        private readonly Object _measureLock;
        private readonly HashSet<IVisualElement> _measureSearch;
        private readonly Queue<IVisualElement> _measureQueue;

        private readonly Object _arrangeLock;
        private readonly HashSet<IVisualElement> _arrangeSearch;
        private readonly Queue<IVisualElement> _arrangeQueue;
    }
}

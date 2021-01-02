using System;

namespace Das.Views
{
    public interface ILayoutQueue
    {
        Boolean HasVisualsNeedingLayout { get; }

        void QueueVisualForMeasure(IVisualElement visual);

        Boolean HasVisualsNeedingMeasure { get; }

        Boolean TryDequeueVisualNeedingMeasure(out IVisualElement visual);

        void RemoveVisualFromMeasureQueue(IVisualElement visual);


        void QueueVisualForArrange(IVisualElement visual);

        Boolean HasVisualsNeedingArrange { get; }

        Boolean TryDequeueVisualNeedingArrange(out IVisualElement visual);

        void RemoveVisualFromArrangeQueue(IVisualElement visual);
    }
}

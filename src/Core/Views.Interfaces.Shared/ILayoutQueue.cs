using System;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views;

public interface ILayoutQueue
{
   void QueueVisualForMeasure(IVisualElement visual);

   Boolean TryDequeueVisualNeedingMeasure(out IVisualElement visual);

   void RemoveVisualFromMeasureQueue(IVisualElement visual);

   void QueueVisualForArrange(IVisualElement visual);

   Boolean TryDequeueVisualNeedingArrange(out IVisualElement visual);

   void RemoveVisualFromArrangeQueue(IVisualElement visual);

   void RemoveVisualFromQueues(IVisualElement visual, 
                               ChangeType type);

   Boolean HasVisualsNeedingLayout { get; }

   Boolean HasVisualsNeedingMeasure { get; }

   Boolean HasVisualsNeedingArrange { get; }
}
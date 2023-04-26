using System;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views.Layout;

public class NullLayoutQueue : ILayoutQueue
{
   private NullLayoutQueue()
   {
   }

   public void QueueVisualForMeasure(IVisualElement visual)
   {
   }

   public Boolean TryDequeueVisualNeedingMeasure(out IVisualElement visual)
   {
      visual = default!;
      return false;
   }

   public void RemoveVisualFromMeasureQueue(IVisualElement visual)
   {
   }

   public void QueueVisualForArrange(IVisualElement visual)
   {
   }

   public Boolean TryDequeueVisualNeedingArrange(out IVisualElement visual)
   {
      visual = default!;
      return false;
   }

   public void RemoveVisualFromArrangeQueue(IVisualElement visual)
   {
   }

   public void RemoveVisualFromQueues(IVisualElement visual,
                                      ChangeType type)
   {
   }

   public Boolean HasVisualsNeedingLayout => false;

   public Boolean HasVisualsNeedingMeasure => false;

   public Boolean HasVisualsNeedingArrange => false;

   public static readonly NullLayoutQueue Instance = new();
}
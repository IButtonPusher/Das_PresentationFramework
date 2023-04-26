using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views.Layout;

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


   public void QueueVisualForMeasure(IVisualElement visual)
   {
      lock (_measureLock)
      {
         if (!_measureSearch.Add(visual))
            return;

         _measureQueue.Enqueue(visual);
      }

      _hasVisualsNeedingMeasure = true;
   }

   public void QueueVisualForArrange(IVisualElement visual)
   {
      lock (_arrangeLock)
      {
         if (!_arrangeSearch.Add(visual))
            return;

         _arrangeQueue.Enqueue(visual);
      }

      _hasVisualsNeedingArrange = true;
   }



   public Boolean HasVisualsNeedingLayout => _hasVisualsNeedingArrange || _hasVisualsNeedingMeasure;

   public Boolean HasVisualsNeedingArrange => _hasVisualsNeedingArrange;


   public Boolean HasVisualsNeedingMeasure => _hasVisualsNeedingMeasure;
     

   public Boolean TryDequeueVisualNeedingMeasure(out IVisualElement visual)
   {
      lock (_measureLock)
      {
         while (true)
         {
            if (_measureQueue.Count == 0)
            {
               visual = default!;
               _hasVisualsNeedingMeasure = false;
               return false;
            }

            visual = _measureQueue.Dequeue();
            if (!_measureSearch.Remove(visual))
               continue;

            _hasVisualsNeedingMeasure = _measureSearch.Count > 0;
            return true;
         }
      }
   }

   public void RemoveVisualFromMeasureQueue(IVisualElement visual)
   {
      lock (_measureLock)
      {
         if (!_measureSearch.Remove(visual))
            return;

         if (_measureSearch.Count == 0)
            _measureQueue.Clear();

      }

      _hasVisualsNeedingMeasure = _measureSearch.Count > 0;
   }

   public void RemoveVisualFromArrangeQueue(IVisualElement visual)
   {
      lock (_arrangeLock)
      {
         if (!_arrangeSearch.Remove(visual))
            return;
      }

      _hasVisualsNeedingArrange = _arrangeSearch.Count > 0;
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
               _hasVisualsNeedingArrange = false;
               return false;
                  
            }

            visual = _arrangeQueue.Dequeue();
            if (!_arrangeSearch.Remove(visual))
               continue;

            _hasVisualsNeedingArrange = _arrangeSearch.Count > 0;
            return true;
         }
      }
   }

   public void RemoveVisualFromQueues(IVisualElement visual,
                                      ChangeType type)
   {
      switch (type)
      {
         case ChangeType.MeasureAndArrange:
            RemoveVisualFromArrangeQueue(visual);
            goto measure;

         case ChangeType.Measure:
            measure:
            RemoveVisualFromMeasureQueue(visual);
            break;

         case ChangeType.Arrange:
            RemoveVisualFromArrangeQueue(visual);
            break;
      }
   }

      

   private readonly Object _arrangeLock;
   private readonly Queue<IVisualElement> _arrangeQueue;
   private readonly HashSet<IVisualElement> _arrangeSearch;

   private readonly Object _measureLock;
   private readonly Queue<IVisualElement> _measureQueue;
   private readonly HashSet<IVisualElement> _measureSearch;
   private Boolean _hasVisualsNeedingArrange;
   private Boolean _hasVisualsNeedingMeasure;
}
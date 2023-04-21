using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Rendering.Geometry;

namespace Das.Views.Rendering
{
   /// <summary>
   ///    Renders a collection of elements vertically or horizontally
   /// </summary>
   public partial class SequentialRenderer : ISequentialRenderer,
                                             IDisposable
   {
      public SequentialRenderer(IVisualCollection visuals,
                                Boolean isWrapContent = false)
      {
         _measureLock = new Object();

         _remainingSize = new RenderSize();
         _currentRenderRectangle = new RenderRectangle();

         //_currentlyRendering = new List<IVisualElement>();
         _visuals = visuals;
         _isWrapContent = isWrapContent;
         ElementsRendered = new Dictionary<IVisualElement, ValueRenderRectangle>();
      }

      public virtual void Dispose()
      {
         //_currentlyRendering.Clear();
         ElementsRendered.Clear();
      }


      public void Clear()
      {
         ElementsRendered.Clear();
      }


      protected virtual ValueSize SetChildSize(IVisualElement child,
                                               RenderRectangle current)
      {
         ElementsRendered[child] = new ValueRenderRectangle(current);

         if (ElementsRendered.Count % 100 == 0)
         {
            Debug.WriteLine($"SequentialRenderer elements rendered: {ElementsRendered.Count}");
         }

         return ValueSize.Empty;
      }

      protected virtual Boolean TryGetElementBounds(IVisualElement child,
                                                    ValueRenderRectangle precedingVisualBounds,
                                                    out ValueRenderRectangle bounds)
      {
         return ElementsRendered.TryGetValue(child, out bounds);
         //ElementsRendered[child];
      }

      //protected virtual ValueRenderRectangle GetElementBounds(IVisualElement child,
      //                                                        ValueRenderRectangle precedingVisualBounds) =>
      //   ElementsRendered[child];


      private Dictionary<IVisualElement, ValueRenderRectangle> ElementsRendered { get; }

      //protected readonly List<IVisualElement> _currentlyRendering;
      protected readonly Boolean _isWrapContent;
      protected readonly Object _measureLock;

      protected readonly IVisualCollection _visuals;
      protected RenderSize _remainingSize;
      protected RenderRectangle _currentRenderRectangle;
   }
}

using System;
using System.Threading.Tasks;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
   public partial class SequentialRenderer
   {
      public virtual ValueSize Measure<TRenderSize>(IVisualElement container,
                                                    Orientations orientation,
                                                    TRenderSize availableSpace,
                                                    IMeasureContext measureContext)
         where TRenderSize : IRenderSize
      {
         lock (_measureLock)
         {
            //_currentlyRendering.Clear();

            var margin = MeasureImpl(container, measureContext, availableSpace,
               orientation, //_currentlyRendering,
               out var maxWidth, out var maxHeight,
               out var totalWidth, out var totalHeight,
               out _);

            return new ValueSize(Math.Max(totalWidth, maxWidth) + margin.Width,
               Math.Max(totalHeight, maxHeight) + margin.Height);
         }
      }

      protected ValueThickness MeasureImpl<TRenderSize>(IVisualElement container,
                                                        IMeasureContext measureContext,
                                                        TRenderSize availableSpace,
                                                        Orientations orientation,
                                                        //List<IVisualElement> currentlyRendering,
                                                        out Double maxWidth,
                                                        out Double maxHeight,
                                                        out Double totalWidth,
                                                        out Double totalHeight,
                                                        out Int32 renderingVisualsCount)
         where TRenderSize : IRenderSize
      {
         _remainingSize.Reset(availableSpace.Width,
            availableSpace.Height, availableSpace.Offset);
         var remainingSize = _remainingSize;
         
         _currentRenderRectangle.Reset();
         var current = _currentRenderRectangle;

         //var remainingSize = new RenderSize(availableSpace.Width,
         //   availableSpace.Height, availableSpace.Offset);
         //var current = new RenderRectangle();

         renderingVisualsCount = 0;

         totalHeight = 0.0;
         totalWidth = 0.0;

         maxWidth = 0.0;
         maxHeight = 0.0;

         foreach (var child in _visuals.GetAllChildren())
         {
            //currentlyRendering.Add(child);
            renderingVisualsCount++;

            current.Size = measureContext.MeasureElement(child, remainingSize);
            var offset = SetChildSize(child, current);
            if (!offset.IsEmpty)
            {
               current.Width += offset.Width;
               current.Height += offset.Height;
            }

            switch (orientation)
            {
               case Orientations.Horizontal:
                  if (current.Height > maxHeight)
                     maxHeight = current.Height;

                  if (_isWrapContent && current.Width + totalWidth > availableSpace.Width
                                     && totalHeight + maxHeight < availableSpace.Height)
                  {
                     maxWidth = Math.Max(maxWidth, totalWidth);
                     totalHeight += maxHeight;

                     current.X = 0;
                     current.Y += maxHeight;
                     maxHeight = totalWidth = 0;
                  }

                  current.X += current.Width;
                  totalWidth += current.Width;
                  remainingSize.Width -= current.Width;
                  break;

               case Orientations.Vertical:
                  if (current.Width > totalWidth)
                     totalWidth = current.Width;

                  if (_isWrapContent && current.Height + totalHeight > availableSpace.Height
                                     && totalWidth + maxWidth < availableSpace.Width)
                  {
                     maxHeight = Math.Max(maxHeight, totalHeight);
                     totalWidth += maxWidth;

                     current.Y = 0;
                     current.X += maxHeight;
                     maxWidth = totalHeight = 0;
                  }

                  current.Y += current.Height;
                  totalHeight += current.Height;
                  remainingSize.Height -= current.Height;
                  break;
            }
         }

         var margin = container.Margin.GetValue(availableSpace);
         return margin;
      }
   }
}

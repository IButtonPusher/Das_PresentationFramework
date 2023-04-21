using Das.Views.Core.Enums;
using Das.Views.Rendering.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Das.Views.Rendering
{
   public partial class SequentialRenderer
   {
      public virtual void Arrange<TRenderRect>(Orientations orientation,
                                               TRenderRect bounds,
                                               IRenderContext renderContext)
         where TRenderRect : IRenderRectangle
      {
         //Debug.WriteLine("arrange sequential renderer");

         var offset = bounds.Location;

         foreach (var kvp in GetRenderables(orientation, bounds))
         {
            if (offset.IsOrigin)
               renderContext.DrawElement(kvp.Key, kvp.Value);
            else
               renderContext.DrawElement(kvp.Key, kvp.Value.Move(offset));
         }

         ElementsRendered.Clear();

         //lock (_measureLock)
         //   _currentlyRendering.Clear();
      }

      protected virtual IEnumerable<KeyValuePair<IVisualElement, ValueRenderRectangle>> GetRenderables(
         Orientations orientation,
         IRenderRectangle bounds)
      {
         lock (_measureLock)
         {
            var current = ValueRenderRectangle.Empty;

            //if (_currentlyRendering.Count == 0)
            //{
            //}

            foreach (var child in _visuals.GetAllChildren())
            {
               //current = GetElementBounds(child, current);
               if (!TryGetElementBounds(child, current, out current))
                  continue;

               current = GetElementRenderBounds(child, current, orientation,
                  bounds);

               yield return new KeyValuePair<IVisualElement, ValueRenderRectangle>(child, current);
            }

            //foreach (var child in _currentlyRendering)
            //{
            //   current = GetElementBounds(child, current);

            //   current = GetElementRenderBounds(child, current, orientation,
            //      bounds);

            //   yield return new KeyValuePair<IVisualElement, ValueRenderRectangle>(child, current);
            //}
         }
      }

      private static ValueRenderRectangle GetElementRenderBounds(IVisualElement child,
                                                                 ValueRenderRectangle current,
                                                                 Orientations orientation,
                                                                 IRenderRectangle bounds)
      {
         var useX = current.X;
         var useY = current.Y;

         switch (orientation)
         {
            case Orientations.Vertical:
               // may need to adjust the X based on alignment

               var useHorzAlign = child.HorizontalAlignment;

               switch (useHorzAlign)
               {
                  case HorizontalAlignments.Right:
                     useX += bounds.Width - current.Width;
                     break;

                  case HorizontalAlignments.Center:
                     useX += (bounds.Width - current.Width) / 2;
                     break;

                  case HorizontalAlignments.Left:
                  case HorizontalAlignments.Default:
                  case HorizontalAlignments.Stretch:

                     break;

                  default:
                     throw new ArgumentOutOfRangeException();
               }

               break;

            case Orientations.Horizontal:

               var useVertAlign = child.VerticalAlignment;

               switch (useVertAlign)
               {
                  case VerticalAlignments.Bottom:
                     useY += bounds.Height - current.Height;
                     break;

                  case VerticalAlignments.Center:
                     useY += (bounds.Height - current.Height) / 2;
                     break;

                  case VerticalAlignments.Top:
                  case VerticalAlignments.Default:
                  case VerticalAlignments.Stretch:

                     break;

                  default:
                     throw new ArgumentOutOfRangeException();
               }

               break;
         }

         current = new ValueRenderRectangle(useX, useY,
            //bounds.Width, //this draws things (picture frame) too wide
            current.Width,
            current.Height, current.Offset);

         return current;
      }
   }
}

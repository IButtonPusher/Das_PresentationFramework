using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Transforms;

namespace Das.Views.Layout
{
   public class BoxModelLayoutTree : IBoxModel
   {
      public BoxModelLayoutTree()
      {
         _currentElementRect = ValueRenderRectangle.Empty;
         RenderPositions = new Dictionary<IVisualElement, ValueCube>();
         _locations = new Stack<ValueRenderRectangle>();
         _locations.Push(_currentElementRect);

         _transforms = new Stack<TransformationMatrix>();

         _netTransform = TransformationMatrix.Identity;
      }

      public void PopTransform<TVisual>(TVisual visual) where TVisual : IVisualElement
      {
         if (visual.Transform.IsIdentity)
            return;

         PopTransform();
      }

      public void PushTransform(TransformationMatrix transform)
      {
         _transforms.Push(transform);
         _netTransform += transform;
      }

      public ValueRenderRectangle PushContentBounds(ValueRenderRectangle bounds)
      {
         var location = _currentElementRect.Location;

         if (location.IsOrigin)
         {
            PushRect(bounds);
            return bounds;
         }

         var b2 = new ValueRenderRectangle(bounds, location);
         //var b2 = new ValueRenderRectangle(bounds.X + location.X, bounds.Y + location.Y,
         //   bounds.Width, bounds.Height, bounds.Offset);


         PushRect(b2);

         return b2;
      }

      public void PopTransform()
      {
         var poppedTransform = _transforms.Pop();
         _netTransform -= poppedTransform;
      }

      public void PopCurrentBox()
      {
         _currentZ--;
         _locations.Pop();
         _currentElementRect = _locations.Peek();
      }

      public ValuePoint2D GetAbsolutePoint<TPoint>(TPoint relativePoint2D,
                                                   Double zoomLevel)
         where TPoint : IPoint2D
      {
         var CurrentLocation = _currentElementRect.Location;

         if (zoomLevel.AreDifferent(1.0))
            return new ValuePoint2D(
               (CurrentLocation.X + relativePoint2D.X) * zoomLevel,
               (CurrentLocation.Y + relativePoint2D.Y) * zoomLevel);

         return new ValuePoint2D(CurrentLocation.X + relativePoint2D.X,
            CurrentLocation.Y + relativePoint2D.Y);
      }

      public ValueRectangle GetAbsoluteRect<TRectangle>(TRectangle relativeRect,
                                                        Double zoomLevel)
         where TRectangle : IRectangle
      {
         var currentLocation = _currentElementRect.Location;

         if (zoomLevel.AreDifferent(1.0) || !_netTransform.IsIdentity)
            return new ValueRectangle(
               (relativeRect.X + currentLocation.X) * zoomLevel * _netTransform.ScaleX +
               zoomLevel * _netTransform.OffsetX,
               (relativeRect.Y + currentLocation.Y) * zoomLevel * _netTransform.ScaleY +
               zoomLevel * _netTransform.OffsetY,
               relativeRect.Width * zoomLevel * _netTransform.ScaleX,
               relativeRect.Height * zoomLevel * _netTransform.ScaleY);

         return new ValueRectangle(currentLocation.X + relativeRect.Left,
            currentLocation.Y + relativeRect.Top,
            relativeRect.Size);
      }


      public ValueRenderRectangle CurrentElementRect => _currentElementRect;

      public ValueRenderRectangle ComputeContentBounds<TRenderRectangle, TThickness>(TRenderRectangle rect,
         TThickness margin,
         TThickness border)
         where TRenderRectangle : IRenderRectangle
         where TThickness : IThickness
      {
         var left = rect.Left;
         var top = rect.Top;
         var w = rect.Width;
         var h = rect.Height;

         if (!margin.IsEmpty)
         {
            left += margin.Left;
            top += margin.Top;
            w -= margin.Width;
            h -= margin.Height;
         }

         if (!_currentElementRect.Offset.IsOrigin)
         {
            left -= _currentElementRect.Offset.X;
            top -= _currentElementRect.Offset.Y;
         }

         if (!border.IsEmpty)
         {
            left += border.Left;
            top += border.Top;
            w -= border.Width;
            h -= border.Height;
         }

         return new ValueRenderRectangle(left, top, w, h, rect.Size.Offset);


         //var left = rect.Left + margin.Left - _currentElementRect.Offset.X;
         //var top = rect.Top + margin.Top - _currentElementRect.Offset.Y;
         //var w = rect.Width - margin.Width;
         //var h = rect.Height - margin.Height;

         //if (!border.IsEmpty)
         //{
         //   left += border.Left;
         //   top += border.Top;
         //   w -= border.Width;
         //   h -= border.Height;
         //}

         //         return new ValueRenderRectangle(left, top, w, h, rect.Size.Offset);
      }

      public void PushTransform<TVisual>(TVisual visual) where TVisual : IVisualElement
      {
         if (visual.Transform.IsIdentity)
            return;

         PushTransform(visual.Transform);
      }


      private void PushRect(ValueRenderRectangle rect)
      {
         _currentZ++;
         _locations.Push(rect);
         _currentElementRect = rect;
      }

      protected Dictionary<IVisualElement, ValueCube> RenderPositions { get; }


      private readonly Stack<ValueRenderRectangle> _locations;
      private readonly Stack<TransformationMatrix> _transforms;

      private ValueRenderRectangle _currentElementRect;
      private Int32 _currentZ;

      private TransformationMatrix _netTransform;
   }
}

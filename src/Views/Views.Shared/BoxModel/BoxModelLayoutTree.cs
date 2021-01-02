using System;
using System.Collections.Generic;
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
            CurrentElementRect = ValueRenderRectangle.Empty;
            RenderPositions = new Dictionary<IVisualElement, ValueCube>();
            _locations = new Stack<ValueRenderRectangle>();
            _locations.Push(CurrentElementRect);

            _transforms = new Stack<TransformationMatrix>();

            _netTransform = TransformationMatrix.Identity;
        }

        //public ValueRenderRectangle PushVisualBox<TRenderRectangle>(TRenderRectangle drawBounds, 
        //                                                            ITransform transform,
        //                                                            IThickness margin, 
        //                                                            IThickness border,
        //                                                            ValueRectangle currentClip) 
        //    where TRenderRectangle : IRenderRectangle
        //{
        //    //_fairyRect ??= new RenderRectangle(0, 0, 0, 0, Point2D.Empty);
            
        //    //_fairyRect.Update(drawBounds,
        //    //    CurrentElementRect.Offset, margin, border);
        //    //var useRect = _fairyRect;

        //    //useRect.Update(drawBounds, CurrentElementRect.Offset, margin, border);

        //    //useRect += CurrentElementRect.Location;
        //    var useRect = ComputeContentBounds(drawBounds, margin, border);
        //    PushRect(useRect);

        //    _transforms.Push(transform.Value);

        //    //SetElementRenderPosition(useRect, visual);
        //    return useRect;
        //}

        public void PushTransform(TransformationMatrix transform)
        {
            _transforms.Push(transform);

            _netTransform += transform;
        }

        public ValueRenderRectangle PushContentBounds(ValueRenderRectangle bounds)
        {
            var location = CurrentElementRect.Location;

            bounds = new ValueRenderRectangle(bounds.X + location.X, bounds.Y + location.Y,
                bounds.Width, bounds.Height, bounds.Offset);

            PushRect(bounds);

            //if (transform is TranslateTransform)
            //{}

            //_transforms.Push(transform.Value);

            //_netTransform += transform.Value;

            return bounds;
        }

        public void PopTransform()
        {
            var poppedTransform = _transforms.Pop();
            _netTransform -= poppedTransform;
        }

        public void PopCurrentBox()
        {
            //var poppedTransform = _transforms.Pop();
            //_netTransform -= poppedTransform;
            _currentZ--;
            _locations.Pop();
            CurrentElementRect = _locations.Peek();
        }

        public ValuePoint2D GetAbsolutePoint<TPoint>(TPoint relativePoint2D,
                                                     Double zoomLevel) 
            where TPoint : IPoint2D
        {
            var CurrentLocation = CurrentElementRect.Location;

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
            var currentLocation = CurrentElementRect.Location;

            if (zoomLevel.AreDifferent(1.0) || !_netTransform.IsIdentity)
                return new ValueRectangle(
                    ((relativeRect.X + currentLocation.X) * zoomLevel * _netTransform.ScaleX) + 
                    (zoomLevel * _netTransform.OffsetX),
                    ((relativeRect.Y + currentLocation.Y) * zoomLevel * _netTransform.ScaleY) + 
                    (zoomLevel * _netTransform.OffsetY),
                    relativeRect.Width * zoomLevel * _netTransform.ScaleX,
                    relativeRect.Height * zoomLevel * _netTransform.ScaleY);

            return new ValueRectangle(currentLocation.X + relativeRect.Left,
                currentLocation.Y + relativeRect.Top,
                relativeRect.Size);


            //if (zoomLevel.AreDifferent(1.0))
            //    return new ValueRectangle(
            //        (relativeRect.X + currentLocation.X) * zoomLevel,
            //        (relativeRect.Y + currentLocation.Y) * zoomLevel,
            //        relativeRect.Width * zoomLevel,
            //        relativeRect.Height * zoomLevel);

            //return new ValueRectangle(currentLocation.X + relativeRect.Left,
            //    currentLocation.Y + relativeRect.Top,
            //    relativeRect.Size);
        }


        public ValueRenderRectangle ComputeContentBounds<TRenderRectangle, TThickness>(TRenderRectangle rect,
                                                                                     TThickness margin,
                                                                                     TThickness border)
            where TRenderRectangle : IRenderRectangle
            where TThickness : IThickness
        {
            var left = rect.Left + margin.Left - CurrentElementRect.Offset.X;
            var top = rect.Top + margin.Top - CurrentElementRect.Offset.Y;
            var w = rect.Width - margin.Width;
            var h = rect.Height - margin.Height;
            //var offset = rect.Offset;

            if (!border.IsEmpty)
            {
                left += border.Left;
                top += border.Top;
                w -= border.Width;
                h -= border.Height;
            }

            return new ValueRenderRectangle(left, top, w, h, rect.Offset);
        }

       

        private void PushRect(ValueRenderRectangle rect)
        {
            _currentZ++;
            _locations.Push(rect);
            CurrentElementRect = rect;
        }

       
        private readonly Stack<ValueRenderRectangle> _locations;
        private readonly Stack<TransformationMatrix> _transforms;

        private TransformationMatrix _netTransform;

        private Int32 _currentZ;
        protected ValueRenderRectangle CurrentElementRect;

        protected Dictionary<IVisualElement, ValueCube> RenderPositions { get; }
    }
}

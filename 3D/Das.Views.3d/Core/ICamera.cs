using System;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Views.Extended
{
    public interface ICamera<out TFrame> : ICamera
        where TFrame : IFrame
    {
        TFrame GetFrame(ISize targetSize);
    }

    public interface ICamera : I3DElement, IMutableViewPerspective
    {
        IPoint3D FocalPoint { get; }

        /// <summary>
        /// Width / Height
        /// </summary>
        Single AspectRatio { get; }

        Single FieldOfView { get; }

        Single NearZenith { get; }

        Single FarZenith { get; }

        void RenderFrame(ISize availableSpace, 
                         IRenderContext renderContext);

        //IFrame GetFrame(ISize targetSize);
    }
}

using System;
using Das.Views.Rendering;

namespace Das.Views.Extended
{
    public interface ICamera<out TFrame> : ICamera
    {
        TFrame GetFrame();
    }

    public interface ICamera : I3dElement, IMutableViewPerspective
    {
        IPoint3d FocalPoint { get; }

        /// <summary>
        /// Width / Height
        /// </summary>
        float AspectRatio { get; }

        float FieldOfView { get; }

        float NearZenith { get; }

        float FarZenith { get; }
    }
}

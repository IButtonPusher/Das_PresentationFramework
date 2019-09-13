using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Views.Extended.Core
{
    public interface ICamera<out TFrame> : ICamera
        where TFrame : IFrame
    {
        new TFrame GetFrame(ISize targetSize);
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

        IFrame GetFrame(ISize targetSize);
    }
}

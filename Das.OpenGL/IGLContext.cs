using System;
using Das.Views.Core.Geometry;

namespace Das.OpenGL
{
    public interface IGLContext
    {
        IntPtr DeviceContextHandle { get; }
        IntPtr RenderContextHandle { get; }

        ISize Size { get; }
    }
}

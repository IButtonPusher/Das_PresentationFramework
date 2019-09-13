using System;

namespace Das.Views.Extended
{
    public interface IVisual3dElement : I3dElement
    {
       IPoint3d[] Vertices { get; }

        Face[] Faces { get; }
    }
}

using System;
using Das.Views.Extended.Core;

namespace Das.Views.Extended
{
    /// <summary>
    /// A collection of three-dimensional points (vertices) and faces that 
    /// are relative to a common position and rotation
    /// </summary>
    public interface IMesh : I3DElement
    {
        IPoint3D[] Vertices { get; }

        Face[] Faces { get; }
    }
}

using System;

namespace Das.Views.Extended
{
    /// <summary>
    /// An element that has a three-dimensional position and rotation
    /// </summary>
    public interface I3DElement
    {
        IPoint3D Position { get; }

        IPoint3D Rotation { get; }

        void Rotate(Single x, Single y, Single z);
    }
}

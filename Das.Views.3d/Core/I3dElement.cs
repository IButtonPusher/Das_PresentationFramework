using System;

namespace Das.Views.Extended
{
    public interface I3dElement
    {
        IPoint3d Position { get; }

        IPoint3d Rotation { get; }

        void Rotate(float x, float y, float z);
    }
}

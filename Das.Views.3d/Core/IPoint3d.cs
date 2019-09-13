using System;

namespace Das.Views.Extended
{
    public interface IPoint3d
    {
        float X { get; }
        float Y { get; }
        float Z { get; }

        IPoint3d Rotate(float x, float y, float z);
    }
}

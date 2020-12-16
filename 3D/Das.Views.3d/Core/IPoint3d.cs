using System;

namespace Das.Views.Extended
{
    public interface IPoint3D : IEquatable<IPoint3D>
    {
        Single X { get; }
        Single Y { get; }
        Single Z { get; }

        IPoint3D Rotate(Single x, Single y, Single z);
    }
}

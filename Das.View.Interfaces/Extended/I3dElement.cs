using System;

namespace Das.Views.Extended
{
    public interface I3dElement
    {
        IPoint3d Position { get; }

        IPoint3d Rotation { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Extended.Core
{
    public readonly struct Transformation3D
    {
        public Transformation3D(ValueVector3 positionOffset, 
                                ValueVector3 rotation, 
                                ValueVector3 scale)
        {
            PositionOffset = positionOffset;
            Rotation = rotation;
            Scale = scale;
        }

        public readonly ValueVector3 PositionOffset;
        public readonly ValueVector3 Rotation;
        public readonly ValueVector3 Scale;
    }
}

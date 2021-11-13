using System;
using System.Threading.Tasks;

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

      public static readonly Transformation3D Identity = new(ValueVector3.Origin,
         ValueVector3.Origin, new ValueVector3(1, 1, 1));

      public readonly ValueVector3 PositionOffset;
      public readonly ValueVector3 Rotation;
      public readonly ValueVector3 Scale;

      public override String ToString()
      {
         return "Pos: " + PositionOffset + " Rot: " + Rotation + " Scale: " + Scale;
      }
   }
}

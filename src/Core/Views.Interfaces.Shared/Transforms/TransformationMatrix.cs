using System;
using Das.Extensions;

namespace Das.Views.Transforms
{
    /// <summary>
    /// A partial 'matrix' for two-dimensional transformations
    /// </summary>
    public readonly struct TransformationMatrix : IEquatable<TransformationMatrix>
    {
        public TransformationMatrix(Double scaleX,
                                    Double skewX,
                                    Double skewY,
                                    Double scaleY,
                                    Double offsetX,
                                    Double offsetY)
                                 //Boolean isIdentity)
        {
            ScaleX = scaleX;
            SkewX = skewX;
            SkewY = skewY;
            ScaleY = scaleY;
            OffsetX = offsetX;
            OffsetY = offsetY;

            _aboutMe = "";
            if (OffsetX.IsNotZero())
                _aboutMe += "Offset-X: " + OffsetX;

            if (OffsetY.IsNotZero())
                _aboutMe += " Offset-Y: " + OffsetY;

            if (String.IsNullOrEmpty(_aboutMe))
                _aboutMe = "Identity";

            IsIdentity = offsetX.IsZero() &&
                         offsetY.IsZero() &&
                         scaleX.AreEqualEnough(1.0) &&
                         scaleY.AreEqualEnough(1.0) &&
                         skewX.IsZero() &&
                         skewY.IsZero();
        }



        public override String ToString()
        {
            return _aboutMe;
        }

        public static TransformationMatrix operator +(TransformationMatrix left,
                                                  TransformationMatrix right)
        {
            if (left.IsIdentity && right.IsIdentity)
                return left;

            return new TransformationMatrix(left.ScaleX * right.ScaleX,
                left.SkewX + right.SkewX,
                left.SkewY + right.SkewY,
                left.ScaleY * right.ScaleY,
                left.OffsetX + right.OffsetX,
                left.OffsetY + right.OffsetY);
        }

        public static TransformationMatrix operator -(TransformationMatrix left,
                                                  TransformationMatrix right)
        {
            if (left.IsIdentity && right.IsIdentity)
                return left;

            return new TransformationMatrix(left.ScaleX / right.ScaleX,
                left.SkewX - right.SkewX,
                left.SkewY - right.SkewY,
                left.ScaleY / right.ScaleY,
                left.OffsetX - right.OffsetX,
                left.OffsetY - right.OffsetY);
        }

        public TransformationMatrix Transition(TransformationMatrix target,
                                               Double percentComplete)
        {
            if (percentComplete >= 1.0)
                return target;

            return new TransformationMatrix(
                Step(ScaleX, target.ScaleX, percentComplete),
                Step(SkewX, target.SkewX, percentComplete),
                Step(SkewY, target.SkewY, percentComplete),
                Step(ScaleY, target.ScaleY, percentComplete),
                Step(OffsetX, target.OffsetX, percentComplete),
                Step(OffsetY, target.OffsetY, percentComplete));
        }

        private static Double Step(Double current,
                                      Double target,
                                      Double percentComplete)
        {
            if (Equals(target, current))
                return target;

            //var scaleXDiff = target - current;
            return current + (target - current) * percentComplete;
            //return scaleXDiff == 0 ? current : current + (scaleXDiff * percentComplete);
        }

        public static TransformationMatrix Identity = new TransformationMatrix(1, 0, 0, 1, 0, 0);

        public readonly Boolean IsIdentity;


        /// <summary>
        /// M11
        /// </summary>
        public readonly Double ScaleX;

        /// <summary>
        /// M12
        /// </summary>
        public readonly Double SkewX;

        /// <summary>
        /// M21, second row first column
        /// </summary>
        public readonly Double SkewY;

        /// <summary>
        /// M22, second row second column
        /// </summary>
        public readonly Double ScaleY;

        /// <summary>
        /// M31, third row first column
        /// </summary>
        public readonly Double OffsetX;

        /// <summary>
        /// M32, third row second column
        /// </summary>
        public readonly Double OffsetY;

        private readonly String _aboutMe;

        public Boolean Equals(TransformationMatrix other)
        {
            return ScaleX.Equals(other.ScaleX) &&
                   SkewX.Equals(other.SkewX) &&
                   SkewY.Equals(other.SkewY) &&
                   ScaleY.Equals(other.ScaleY) &&
                   OffsetX.Equals(other.OffsetX) &&
                   OffsetY.Equals(other.OffsetY);
        }

        public override bool Equals(object? obj)
        {
            return obj is TransformationMatrix other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ScaleX.GetHashCode();
                hashCode = (hashCode * 397) ^ SkewX.GetHashCode();
                hashCode = (hashCode * 397) ^ SkewY.GetHashCode();
                hashCode = (hashCode * 397) ^ ScaleY.GetHashCode();
                hashCode = (hashCode * 397) ^ OffsetX.GetHashCode();
                hashCode = (hashCode * 397) ^ OffsetY.GetHashCode();
                return hashCode;
            }
        }
    }
}

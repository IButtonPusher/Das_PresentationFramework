using System;
using Das.Extensions;

namespace Das.Views.Transforms
{
    public class ScaleTransform : ITransform
    {
        public Double ScaleX { get; }

        public Double ScaleY { get; }

        public ScaleTransform(Double scale) 
            : this(scale, scale)
        {
            
        }

        public ScaleTransform(Double scaleX,
                              Double scaleY)
        {
            ScaleX = scaleX;
            ScaleY = scaleY;

            IsIdentity = ScaleX.AreEqualEnough(1.0) && ScaleY.AreEqualEnough(1.0);

            Value = IsIdentity 
                ? ValueTranslation.Identity 
                : new ValueTranslation(scaleX, 0, 0, scaleY, 0, 0);
        }

        public Boolean IsIdentity { get; }

        public ValueTranslation Value { get; }
    }
}

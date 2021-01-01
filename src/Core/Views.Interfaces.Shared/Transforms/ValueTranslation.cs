using System;

namespace Das.Views.Transforms
{
    /// <summary>
    /// A partial 'matrix' for two-dimensional transformations
    /// </summary>
    public readonly struct ValueTranslation
    {
        public ValueTranslation(Double scaleX, 
                                Double skewX, 
                                Double skewY, 
                                Double scaleY, 
                                Double offsetX, 
                                Double offsetY)
        {
            ScaleX = scaleX;
            SkewX = skewX;
            SkewY = skewY;
            ScaleY = scaleY;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public static ValueTranslation Identity = new ValueTranslation(1, 0, 0, 1, 0, 0);


        public readonly Double ScaleX;

        public readonly Double SkewX;

        /// <summary>
        /// AKA M21, second row first column
        /// </summary>
        public readonly Double SkewY;
        
        /// <summary>
        /// AKA M22, second row second column
        /// </summary>
        public readonly Double ScaleY;

        /// <summary>
        /// AKA M31, third row first column
        /// </summary>
        public readonly Double OffsetX;
        
        /// <summary>
        /// AKA M32, third row second column
        /// </summary>
        public readonly Double OffsetY;
    }
}

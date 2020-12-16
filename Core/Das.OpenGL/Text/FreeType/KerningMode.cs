﻿namespace Das.OpenGL.Text.FreeType
{
    public enum KerningMode : uint
    {
        /// <summary>
        /// Return scaled and grid-fitted kerning distances.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Return scaled but un-grid-fitted kerning distances.
        /// </summary>
        Unfitted,

        /// <summary>
        /// Return the kerning vector in original font units.
        /// </summary>
        Unscaled
    }
}

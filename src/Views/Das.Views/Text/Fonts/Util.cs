using System;

namespace Das.Views.Text.Fonts
{
    public static class Util
    {
        internal const int nullOffset = -1;

        private static readonly string[] SupportedExtensions = new string[5]
        {
            ".COMPOSITEFONT",
            ".OTF",
            ".TTC",
            ".TTF",
            ".TTE"
        };

        //public static float PixelsPerDip => (float) Util.Dpi / 96f;
    }
}

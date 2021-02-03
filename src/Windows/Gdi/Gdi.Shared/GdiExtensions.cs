using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Gdi.Shared
{
    public static class GdiExtensions
    {
        public static Graphics GetSmoothGraphics(this Image image)
        {
            var g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            return g;
        }
    }
}

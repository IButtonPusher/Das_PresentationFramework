using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

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

        public static Control? FindControl(this Control.ControlCollection controlCollection,
                                           IntPtr handle)
        {
            foreach (var c in controlCollection)
            {
                if (!(c is Control ctrl))
                    continue;

                if (Equals(ctrl.Handle, handle))
                    return ctrl;
            }

            return default;
        }
    }
}

﻿using System;
using System.Drawing;
using Das.Gdi.Core;
using Das.Views.Core.Drawing;
using Das.Views.Measuring;
using Font = Das.Views.Core.Writing.Font;
using Size = Das.Views.Core.Geometry.Size;

namespace Das.Gdi
{
    public class GdiMeasureContext : BaseMeasureContext
    {
        public GdiMeasureContext()
        {
            var bmp = new Bitmap(1, 1);
            Graphics = Graphics.FromImage(bmp);
        }

        internal Graphics Graphics { get; }

        public override Size MeasureString(string s, Font font)
        {
            var useFont = TypeConverter.GetFont(font);
            var sz = Graphics.MeasureString(s, useFont);
            return new Size(sz.Width, sz.Height);
        }

        public override Size MeasureImage(IImage img) => new Size(img.Width, img.Height);
    }
}
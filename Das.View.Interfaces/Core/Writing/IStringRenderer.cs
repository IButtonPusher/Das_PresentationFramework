﻿using System;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Views.Core.Writing
{
    public interface IStringRenderer
    {
        void DrawString(String text, IFont font, IBrush brush, IPoint point);
    }
}

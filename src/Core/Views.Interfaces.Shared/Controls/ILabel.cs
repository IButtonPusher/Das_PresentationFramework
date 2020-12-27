﻿using System;
using Das.Views.Core.Drawing;

namespace Das.Views.Controls
{
    public interface ILabel : IVisualElement
    {
        String Text { get; set; }
        
        IBrush? TextBrush { get; set; }
    }
}

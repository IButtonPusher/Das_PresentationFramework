﻿using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public interface IRenderedVisual
    {
        IVisualElement Element { get; }

        ICube Position { get; }
    }
}
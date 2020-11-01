﻿using System;
using Das.Views.Rendering;

namespace Das.Views.Controls
{
    public interface IVisualSurrogateProvider
    {
        void EnsureSurrogate(ref IVisualElement element);
    }
}

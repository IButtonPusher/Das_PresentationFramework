﻿using Das.Views.Rendering;

namespace Das.Views.Styles
{
    // ReSharper disable once UnusedTypeParameter
    public class TypeStyle<T> : TypeStyle where T : IVisualElement
    {
    }

    public abstract class TypeStyle : Style
    {
    }
}
using System;
using System.Collections.Generic;

namespace Das.Views.Styles
{
    public interface IStyleSheet : IStyle
    {
        IDictionary<Type, IStyle> VisualTypeStyles { get; }
    }
}

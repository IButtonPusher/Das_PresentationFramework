using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Styles
{
    public interface IStyleSheet : IStyle
    {
        IDictionary<Type, IStyle> VisualTypeStyles { get; }
    }
}
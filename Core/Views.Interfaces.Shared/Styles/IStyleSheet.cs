using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Styles
{
    /// <summary>
    /// A collection of style rules (zb from a single .css file)
    /// </summary>
    public interface IStyleSheet : IStyle
    {
        IEnumerable<IStyleRule> Rules { get; }

        Boolean TryGetValue<T>(IVisualElement visual,
                               out T value);

        IDictionary<Type, IStyleSheet> VisualTypeStyles { get; }
    }
}
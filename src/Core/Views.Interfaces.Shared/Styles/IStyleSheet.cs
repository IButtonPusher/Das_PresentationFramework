using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Das.Views.Styles;

/// <summary>
///     A collection of style rules (zb from a single .css file)
/// </summary>
public interface IStyleSheet : IStyle
{
   IEnumerable<IStyleRule> Rules { get; }

   IDictionary<Type, IStyleSheet> VisualTypeStyles { get; }

   /// <summary>
   ///     Adds rules if the current style sheet doesn't already contain an equivalent,
   ///     and returns a new style sheet.
   /// </summary>
   [Pure]
   IStyleSheet AddDefaultRules(IEnumerable<IStyleRule> rules);

   Boolean TryGetValue<T>(IVisualElement visual,
                          out T value);

   Boolean IsEmpty { get; }
}
using System;
using System.Collections.Generic;

namespace Das.Views.Styles
{
    public interface IStyleRule : IEquatable<IStyleRule>
    {
        /// <summary>
        /// Filters for which visual(s) will be styled by this rule
        /// </summary>
        IStyleSelector Selector { get; }
        
        /// <summary>
        /// The collection of properties and the values to set for them
        /// </summary>
        IEnumerable<IStyleDeclaration> Declarations { get; }
    }
}

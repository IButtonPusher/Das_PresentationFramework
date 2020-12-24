using System;
using System.Collections.Generic;

namespace Das.Views.Styles
{
    public interface ICascadingStyle
    {
        IEnumerable<IStyleRule> Rules { get; }
    }
}

using System;
using System.Collections.Generic;

namespace Das.Views.Styles.Application
{
    public interface IAppliedStyle : IStyleApplication
    {
        IStyleSheet StyleTemplate { get; }

        IEnumerable<IAppliedStyleRule> AppliedRules { get; }
    }
}

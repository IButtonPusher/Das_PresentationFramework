using System;
using System.Collections.Generic;

namespace Das.Views.Styles.Application
{
    public interface IAppliedStyleRule : IStyleApplication
    {
        IStyleRule RuleTemplate { get; }

        IEnumerable<IStyleCondition> Conditions { get; }

        IEnumerable<IStyleValueAssignment> Assignments { get; }
    }
}

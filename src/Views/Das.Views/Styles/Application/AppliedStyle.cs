using System;
using System.Collections.Generic;

namespace Das.Views.Styles.Application
{
    public class AppliedStyle : IAppliedStyle
    {
        public AppliedStyle(IStyleSheet styleTemplate)
        {
            StyleTemplate = styleTemplate;
            AppliedRules = new List<IAppliedStyleRule>();
        }

        public IStyleSheet StyleTemplate { get; }

        public List<IAppliedStyleRule> AppliedRules { get; }

        IEnumerable<IAppliedStyleRule> IAppliedStyle.AppliedRules => AppliedRules;

        public void Execute()
        {
            foreach (var rule in AppliedRules)
            {
                rule.Execute();
            }
        }
    }
}

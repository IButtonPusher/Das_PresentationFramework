using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Styles.Application
{
    public class AppliedStyleRule : IAppliedStyleRule
    {
        public AppliedStyleRule(IStyleRule ruleTemplate)
        {
            RuleTemplate = ruleTemplate;
            Conditions = new List<IStyleCondition>();
            Assignments = new List<IStyleValueAssignment>();
        }

        public void Execute()
        {
            foreach (var condition in Conditions)
                if (!condition.CanExecute())
                    return;

            foreach (var assignment in Assignments)
                assignment.Execute();
        }

        public IStyleRule RuleTemplate { get; }

        IEnumerable<IStyleCondition> IAppliedStyleRule.Conditions => Conditions;

        IEnumerable<IStyleValueAssignment> IAppliedStyleRule.Assignments => Assignments;

        public List<IStyleValueAssignment> Assignments { get; }

        public List<IStyleCondition> Conditions { get; }

        public override String ToString()
        {
            return "Assigned: " + RuleTemplate + " Count: " + Assignments.Count;
        }
    }
}
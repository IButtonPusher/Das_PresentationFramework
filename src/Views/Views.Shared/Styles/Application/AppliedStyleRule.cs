using System;
using System.Collections.Generic;
using System.Linq;
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

        public Boolean IsFilteringOnVisualState()
        {
            return RuleTemplate.Selector.IsFilteringOnVisualState();
        }

        public void EnsureDefaultAssignments(AppliedStyleRule other)
        {
            foreach (var filteredAssignment in other.Assignments)
            {
                if (filteredAssignment is PseudoVisualAssignment pseudo)
                {
                    var myPseudo = Assignments.OfType<PseudoVisualAssignment>().FirstOrDefault(a => a.Equals(a));

                    if (myPseudo == null)
                        continue;


                    foreach (var subAssignment in pseudo.Assignments)
                    {
                        var hasOverlap = myPseudo.DoOverlap(subAssignment);

                        if (!hasOverlap)
                        {
                            var defaultAssignment = GetDefaultAssignment(subAssignment);
                            myPseudo.AddAssignment(defaultAssignment);
                        }

                    }



                }
                else
                {
                    if (!HasOverlappingAssignment(filteredAssignment))
                        AddDefaultAssignment(filteredAssignment);
                }
            }
        }

        public Boolean HasOverlappingAssignment(IStyleValueAssignment assignment)
        {
            foreach (var existing in Assignments)
            {
                if (existing.DoOverlap(assignment))
                    return true;
            }

            return false;
        }

        private static IStyleValueAssignment GetDefaultAssignment(IStyleValueAssignment exampleAssignment)
        {
            if (!(exampleAssignment is IPropertyValueAssignment propertyValue))
                throw new NotImplementedException();

            var applied = new AppliedValueAssignment(propertyValue.Visual, propertyValue.Property,
                propertyValue.Property.DefaultValue);
            return applied;
        }

        public void AddDefaultAssignment(IStyleValueAssignment exampleAssignment)
        {
            var applied = GetDefaultAssignment(exampleAssignment);
            Assignments.Add(applied);
        }

        public void Execute(Boolean isUpdate)
        {
            foreach (var condition in Conditions)
                if (!condition.CanExecute())
                    return;

            foreach (var assignment in Assignments)
                assignment.Execute(isUpdate);
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
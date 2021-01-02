using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Rendering;
using Das.Views.Styles.Selectors;

namespace Das.Views.Styles.Application
{
    public class PseudoVisualAssignment : IStyleValueAssignment,
                                          IEquatable<PseudoVisualAssignment>
    {
        public PseudoVisualAssignment(IVisualElement toVisual,
                                      ContentAppendType appendType,
                                      IStyleRule rule,
                                      IVisualBootstrapper visualBootstrapper,
                                      IVisualLineage visualLineage,
                                      BuildAssignments assignmentsBuilder)
        {
            Visual = toVisual;
            _appendType = appendType;
            ILabel applyTo;

            switch (appendType)
            {
                case ContentAppendType.Before:
                    if (!(toVisual.BeforeLabel is { } beforeLabel))
                    {
                        applyTo = visualBootstrapper.Instantiate<Label>();
                        toVisual.BeforeLabel = applyTo;
                    }
                    else applyTo = beforeLabel;

                    break;

                case ContentAppendType.After:
                    if (!(toVisual.AfterLabel is { } afterLabel))
                    {
                        applyTo = visualBootstrapper.Instantiate<Label>();
                        toVisual.AfterLabel = applyTo;
                    }
                    else applyTo = afterLabel;

                    break;

                default:
                    throw new NotImplementedException();
            }

            var res = assignmentsBuilder(applyTo, visualLineage,
                AllStyleSelector.Instance, rule.Declarations);

            _assignments = new List<IStyleValueAssignment>(res);
        }

        public void AddAssignment(IStyleValueAssignment assignment)
        {
            _assignments.Add(assignment);
        }

        public void Execute(Boolean isUpdate)
        {
            foreach (var assignment in _assignments)
                assignment.Execute(isUpdate);
        }

        public Boolean DoOverlap(IStyleValueAssignment other)
        {
            if (other is PseudoVisualAssignment pseudo)
            {
                foreach (var pAssign in pseudo._assignments)
                {
                    if (DoOverlap(pAssign))
                        return true;
                }

                return false;
            }

            foreach (var assignment in _assignments)
                if (assignment.DoOverlap(other))
                    return true;

            return false;
        }

        public Boolean Equals(PseudoVisualAssignment other)
        {
            return Equals(Visual, other.Visual) &&
                   Equals(_appendType, other._appendType);
        }

        public override String ToString()
        {
            return "Pseudo " + Visual + "::" + _appendType;
        }

        private readonly ContentAppendType _appendType;

        private readonly List<IStyleValueAssignment> _assignments;

        public IEnumerable<IStyleValueAssignment> Assignments => _assignments;

        public IVisualElement Visual { get; }
    }
}
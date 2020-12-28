using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Rendering;
using Das.Views.Styles.Selectors;

namespace Das.Views.Styles.Application
{
    public class PseudoVisualAssignment : IStyleValueAssignment
    {
        public PseudoVisualAssignment(IVisualElement toVisual,
                                      ContentAppendType appendType,
                                      IStyleRule rule,
                                      IVisualBootstrapper visualBootstrapper,
                                      IVisualLineage visualLineage,
                                      BuildAssignments assignmentsBuilder)
        {
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

        public void Execute()
        {
            foreach (var assignment in _assignments) assignment.Execute();
        }

        private readonly List<IStyleValueAssignment> _assignments;
    }
}
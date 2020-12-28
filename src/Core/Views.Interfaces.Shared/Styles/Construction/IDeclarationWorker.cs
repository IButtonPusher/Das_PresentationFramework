using System.Collections.Generic;
using Das.Views.Rendering;
using Das.Views.Styles;
using Das.Views.Styles.Application;

namespace Das.Views.Construction.Styles
{
    public interface IDeclarationWorker
    {
        IEnumerable<IStyleValueAssignment> BuildStyleValueAssignments(IVisualElement visual,
                                                                     IVisualLineage visualLineage,
                                                                     IStyleRule rule);
    }
}
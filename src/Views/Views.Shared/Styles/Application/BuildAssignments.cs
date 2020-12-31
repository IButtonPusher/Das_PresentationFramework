using System;
using System.Collections.Generic;
using Das.Views.Rendering;

namespace Das.Views.Styles.Application
{
    public delegate IEnumerable<IStyleValueAssignment> BuildAssignments(IVisualElement visual,
                                                           IVisualLineage visualLineage,
                                                           IStyleSelector selector,
                                                           IEnumerable<IStyleDeclaration> declarations);
}

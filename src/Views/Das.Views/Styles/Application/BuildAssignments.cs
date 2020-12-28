using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.Rendering;

namespace Das.Views.Styles.Application
{
    public delegate IEnumerable<IStyleValueAssignment> BuildAssignments(IVisualElement visual,
                                                           IVisualLineage visualLineage,
                                                           IStyleSelector selector,
                                                           IEnumerable<IStyleDeclaration> declarations);
}

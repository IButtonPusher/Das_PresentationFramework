using System;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views.Construction
{
    public delegate Task ApplyVisualStyles(IVisualElement visual,
                                           IAttributeDictionary attributeDictionary,
                                           IVisualLineage visualLineage,
                                           IViewInflater viewInflater,
                                           IVisualBootstrapper visualBootstrapper);
}

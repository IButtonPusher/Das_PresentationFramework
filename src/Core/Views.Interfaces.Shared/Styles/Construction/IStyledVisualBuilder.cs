using System;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views.Construction.Styles
{
    /// <summary>
    /// Finds and applies styles to visuals
    /// </summary>
    public interface IStyledVisualBuilder
    {
        //Task ApplyStylesToVisualAsync(IVisualElement visual,
        //                         String? styleClassName,
        //                         IVisualLineage visualLineage);

        Task ApplyStylesToVisualAsync(IVisualElement visual,
                                      IAttributeDictionary attributeDictionary,
                                      IVisualLineage visualLineage,
                                      IViewInflater viewInflater);
    }
}

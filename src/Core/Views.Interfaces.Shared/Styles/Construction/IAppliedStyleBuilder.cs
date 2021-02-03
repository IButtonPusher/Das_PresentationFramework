using System;
using System.Threading.Tasks;
using Das.Views.Construction;
using Das.Views.Rendering;
using Das.Views.Styles.Application;

namespace Das.Views.Styles.Construction
{
    public interface IAppliedStyleBuilder
    {
        IAppliedStyle? BuildAppliedStyle(IStyleSheet style,
                                         IVisualElement visual,
                                         IVisualLineage visualLineage,
                                         IVisualBootstrapper visualBootstrapper);

        Task ApplyVisualStylesAsync(IVisualElement visual,
                                    IAttributeDictionary attributeDictionary,
                                    IVisualLineage visualLineage,
                                    IVisualBootstrapper visualBootstrapper);

        Task ApplyVisualStylesAsync(IVisualElement visual,
                                    IAttributeDictionary attributeDictionary,
                                    IVisualLineage visualLineage,
                                    IViewInflater viewInflater,
                                    IVisualBootstrapper visualBootstrapper);

        /// <summary>
        /// Applied styles for the visual's Type and inherited types.
        /// Does not apply styles based on a class or the Visual's Style property value
        /// </summary>
        ///  <see cref="IVisualElement.Style"/>
        void ApplyVisualCoreStyles(IVisualElement visual,
                                   IVisualBootstrapper visualBootstrapper);

        IVisualStyleProvider StyleProvider { get; }
    }
}

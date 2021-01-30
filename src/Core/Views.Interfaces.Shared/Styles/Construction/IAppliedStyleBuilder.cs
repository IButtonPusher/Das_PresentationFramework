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

        void ApplyVisualCoreStyles(IVisualElement visual,
                                   IVisualBootstrapper visualBootstrapper);

        IVisualStyleProvider StyleProvider { get; }
    }
}

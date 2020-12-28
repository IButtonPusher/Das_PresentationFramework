using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Construction.Styles;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Rendering;
using Das.Views.Styles;
using Das.Views.Styles.Construction;
using Das.Views.Templates;

namespace Das.Views.Construction
{
    public class StyledVisualBuilder : IStyledVisualBuilder
    {
        public StyledVisualBuilder(IVisualBootstrapper visualBootstrapper,
                                   IVisualStyleProvider styleProvider,
                                   IPropertyProvider propertyProvider,
                                   Dictionary<IVisualElement, ValueCube> renderPositions,
                                   IAppliedStyleBuilder appliedStyleBuilder)
        {
            _visualBootstrapper = visualBootstrapper;
            _styleProvider = styleProvider;
            _propertyProvider = propertyProvider;
            _renderPositions = renderPositions;
            _appliedStyleBuilder = appliedStyleBuilder;
        }


        public async Task ApplyStylesToVisualAsync(IVisualElement visual,
                                                   String? styleClassName,
                                                   IVisualLineage visualLineage)
        {
            if (!String.IsNullOrEmpty(styleClassName))
            {
                var classStyles = _styleProvider.GetStylesByClassNameAsync(styleClassName!);

                await foreach (var style in classStyles)
                {
                }
            }
        }

        public async Task ApplyStylesToVisualAsync(IVisualElement visual,
                                                   IAttributeDictionary attributeDictionary,
                                                   IVisualLineage visualLineage,
                                                   IViewInflater viewInflater)
        {
            if (attributeDictionary.TryGetAttributeValue("class", out var className))
            {
                await ApplyStylesToVisualAsync(visual, className, visualLineage);
                return;
            }

            if (attributeDictionary.TryGetAttributeValue("Style", out var styleName))
            {
                var style = await _styleProvider.GetStyleByNameAsync(styleName);
                if (style == null)
                    return;

                var applyingRules = new StyledVisualWorker(style, _propertyProvider, _visualBootstrapper);

                applyingRules.TrySetVisualStyle(visual, style);

                if (applyingRules.TryGetVisualTemplate(out var visualTemplate) &&
                    visualTemplate is DeferredVisualTemplate deferred)
                {
                    //var contentVisual = await viewInflater.GetVisualAsync(deferred.MarkupNode,
                    //    visual.GetType(), visualLineage, applyingRules.ApplyStylesToVisualAsync);

                    var contentVisual = await viewInflater.GetVisualAsync(deferred.MarkupNode,
                        visual.GetType(), visualLineage, _appliedStyleBuilder.ApplyVisualStylesAsync);

                    visual.Template = new VisualTemplate
                    {
                        Content = contentVisual
                    };

                    //control template's data context should be the control being templated....?
                    if (contentVisual is IBindableElement bindable)
                        bindable.DataContext = visual;

                    visualLineage.AssertPopVisual(contentVisual);

                    var appliedStyle = _appliedStyleBuilder.BuildAppliedStyle(style, visual, visualLineage);
                }
                else
                {
                    applyingRules.ApplyStyleValuesToVisual(visual, attributeDictionary, visualLineage);
                }
            }
        }

        private readonly IVisualBootstrapper _visualBootstrapper;
        private readonly IVisualStyleProvider _styleProvider;
        private readonly IPropertyProvider _propertyProvider;
        private readonly Dictionary<IVisualElement, ValueCube> _renderPositions;
        private readonly IAppliedStyleBuilder _appliedStyleBuilder;
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Construction.Styles;
using Das.Views.Controls;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Construction
{
    public class StyledVisualWorker
    {
        public StyledVisualWorker(IStyleSheet styleSheet,
                                  IPropertyProvider propertyProvider,
                                  IVisualBootstrapper visualBootstrapper)
        {
            _propertyProvider = propertyProvider;
            _visualBootstrapper = visualBootstrapper;
            _rules = new List<IStyleRule>(styleSheet.Rules);

            //_declarationWorker = new DeclarationWorker(renderPositions, visualBootstrapper);
        }

        public async Task ApplyStylesToVisualAsync(IVisualElement visual,
                                                   IAttributeDictionary attributeDictionary,
                                                   IVisualLineage visualLineage,
                                                   IViewInflater viewInflater)
        {
            var applicableRules = ApplyStyleValuesToVisual(visual, attributeDictionary, visualLineage);

            TrySetVisualStyle(visual, new StyleSheet(applicableRules));

            await Task.CompletedTask;
        }
        
        public List<IStyleRule> ApplyStyleValuesToVisual(IVisualElement visual,
                                                         IAttributeDictionary attributeDictionary,
                                                         IVisualLineage visualLineage)
        {
            TrySetVisualClass(visual, attributeDictionary);

            var applicableRules = new List<IStyleRule>();

            foreach (var rule in _rules)
            {
                if (TryApplyRuleToVisual(visual, rule, visualLineage))
                    applicableRules.Add(rule);
            }

            return applicableRules;
        }

        private void TrySetVisualClass(IVisualElement visual,
                                       IAttributeDictionary attributeDictionary)
        {
            if (attributeDictionary.TryGetAttributeValue("class", out var className))
            {
                var propAccessor = _propertyProvider.GetPropertyAccessor(visual.GetType(),
                    nameof(IVisualElement.Class));
                if (propAccessor.CanWrite)
                {
                    Object oVisual = visual;
                    propAccessor.SetPropertyValue(ref oVisual, className);
                }
            }
        }

        public void TrySetVisualStyle(IVisualElement visual,
                                       IStyleSheet styleSheet)
        {
            var propAccessor = _propertyProvider.GetPropertyAccessor(visual.GetType(),
                nameof(IVisualElement.Style));
            if (propAccessor.CanWrite)
            {
                Object oVisual = visual;
                propAccessor.SetPropertyValue(ref oVisual, styleSheet); 
            }
        }

        public Boolean TryGetVisualTemplate(out IVisualTemplate template)
        {
            var items = from r in _rules.OfType<DependencyPropertyValueRule>()
                where r.Selector.Property.Name == nameof(IVisualElement.Template)
                select r;

            var goodRule = items.FirstOrDefault();

            if (goodRule?.Declaration.Value is IVisualTemplate visualTemplate)
            {
                template = visualTemplate;
                return true;
            }

            template = default!;
            return false;
        }


        private Boolean TryApplyRuleToVisual(IVisualElement visual,
                                             IStyleRule rule,
                                             IVisualLineage visualLineage)
        {
            var applicableVisuals = SelectorWorker.GetSelectableVisuals(visual,
                rule.Selector, visualLineage);

            foreach (var applicable in applicableVisuals)
            {
                Debug.WriteLine("Rule: " + rule + " applies to " + applicable);


                if (rule.Selector.TryGetContentAppendType(out var contentAppend))
                {
                    AddPseudoVisual(applicable, contentAppend, rule, visualLineage);
                    //return true;
                }
                else
                {
                    foreach (var declaration in rule.Declarations)
                    {
                        ApplyDeclarationToVisual(applicable, visualLineage,
                            declaration, rule.Selector);
                    }
                }

            }

            return true;
        }

        private void AddPseudoVisual(IVisualElement toVisual,
                                     ContentAppendType appendType,
                                     IStyleRule rule,
                                     IVisualLineage visualLineage)
        {
            switch (appendType)
            {
                case ContentAppendType.Before:
                    if (!(toVisual.BeforeLabel is { } beforeLabel))
                    {
                        beforeLabel = _visualBootstrapper.Instantiate<Label>();
                        toVisual.BeforeLabel = beforeLabel;
                    }

                    foreach (var declaration in rule.Declarations)
                    {
                        ApplyDeclarationToVisual(beforeLabel, visualLineage, 
                            declaration, rule.Selector);
                    }

                    break;

                case ContentAppendType.After:
                    if (!(toVisual.AfterLabel is { } afterLabel))
                    {
                        afterLabel = _visualBootstrapper.Instantiate<Label>();
                        toVisual.AfterLabel = afterLabel;
                    }

                    foreach (var declaration in rule.Declarations)
                    {
                        ApplyDeclarationToVisual(afterLabel, visualLineage, 
                            declaration, rule.Selector);
                    }
                    break;
            }
        }

        private static void ApplyDeclarationToVisual(IVisualElement visual,
                                                     IVisualLineage visualLineage,
                                                     IStyleDeclaration declaration,
                                                     IStyleSelector selector)
        {
            if (visual.TryGetDependencyProperty(declaration.Property, //declarationValue, 
                out var dependencyProperty))
            {
                Debug.WriteLine("Setting " + visual.GetType().Name + "->" + dependencyProperty +
                                " = " + declaration);

                ApplyDeclarationToDependencyProperty(visual, visualLineage,
                    dependencyProperty, declaration, selector);
            }
            else
            {
                Debug.WriteLine("No dependency property found for " + declaration.Property +
                                " on " + visual);
            }
        }

        private static void ApplyDeclarationToDependencyProperty(IVisualElement visual,
                                                                 IVisualLineage visualLineage,
                                                                 IDependencyProperty property,
                                                                 IStyleDeclaration declaration,
                                                                 IStyleSelector selector)
        {
            if (selector.IsFilteringOnVisualState())
            {}

            var declarationValue = DeclarationWorker.GetDeclarationValue(visual, 
                declaration, visualLineage);

            switch (declarationValue)
            {
                case Func<IVisualElement, Object?> computed:
                    property.SetComputedValueFromStyle(visual, computed);
                    break;
                
                default:
                    property.SetValueFromStyle(visual, declarationValue);
                    break;
            }
        }

        private readonly IPropertyProvider _propertyProvider;
        private readonly IVisualBootstrapper _visualBootstrapper;
        //private readonly IDeclarationWorker _declarationWorker;

        private readonly List<IStyleRule> _rules;
    }
}
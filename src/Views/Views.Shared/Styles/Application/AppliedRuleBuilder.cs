using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Construction;
using Das.Views.Construction.Styles;
using Das.Views.Controls;
using Das.Views.DataBinding;
using Das.Views.Input;
using Das.Views.Layout;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles.Application;
using Das.Views.Styles.Construction;
using Das.Views.Styles.Selectors;
using Das.Views.Templates;

namespace Das.Views.Styles
{
    public class AppliedRuleBuilder : IAppliedStyleBuilder
    {
        public AppliedRuleBuilder(IVisualStyleProvider styleProvider,
                                  IDeclarationWorker declarationWorker,
                                  IPropertyProvider propertyProvider)
        {
            _styleProvider = styleProvider;
            StyleProvider = styleProvider;
            _declarationWorker = declarationWorker;
            _propertyProvider = propertyProvider;
        }

        public IAppliedStyle? BuildAppliedStyle(IStyleSheet style,
                                                IVisualElement visual,
                                                IVisualLineage visualLineage,
                                                IVisualBootstrapper visualBootstrapper)
        {
            var appliedStyle = new AppliedStyle(style);

            foreach (var rule in style.Rules)
            {
                var appliedRule = BuildAppliedRule(appliedStyle, rule, visual, visualLineage,
                    visualBootstrapper);
                if (appliedRule == null)
                    continue;

                appliedStyle.AddAppliedRule(appliedRule);
            }

            appliedStyle.EnsureInverseForFilteredSelectors();

            TrySetVisualStyle(visual, appliedStyle);

            if (!appliedStyle.HasAppliedRules)
                return default;

            return appliedStyle;
        }

        public async Task ApplyVisualStylesAsync(IVisualElement visual, 
                                                 IAttributeDictionary attributeDictionary,
                                                 IVisualLineage visualLineage,
                                                 IVisualBootstrapper visualBootstrapper)
        {
            var useStyle = await _styleProvider.GetStyleForVisualAsync(visual, attributeDictionary);
            if (useStyle == null)
                return;

            TrySetVisualClass(visual, attributeDictionary);

            var appliedStyle = BuildAppliedStyle(useStyle, visual, visualLineage, visualBootstrapper);

            if (appliedStyle != null)
            {
                TrySetVisualStyle(visual, appliedStyle);
                appliedStyle.Execute(false);
            }
        }

        private async Task EnsureStyleVisualTemplates(IVisualElement visual,
            IStyleSheet style,
            IViewInflater viewInflater,
            IVisualLineage visualLineage,
            IVisualBootstrapper visualBootstrapper)
        {
            if (!TryGetVisualTemplate(style, out var visualTemplate) ||
                !(visualTemplate is DeferredVisualTemplate deferred))
            {
                return;
            }

            var contentVisual = await viewInflater.GetVisualAsync(deferred.MarkupNode,
                visual.GetType(), visualLineage, ApplyVisualStylesAsync);

            visual.Template = new VisualTemplate
            {
                Content = contentVisual
            };

            //control template's data context should be the control being templated....?
            if (contentVisual is IBindableElement bindable)
                bindable.DataContext = visual;

            visualLineage.AssertPopVisual(contentVisual);

            BuildAppliedStyle(style, visual, visualLineage, visualBootstrapper);
        }

        private static Boolean TryGetVisualTemplate(IStyleSheet style,
                                                    out IVisualTemplate template)
        {
            foreach (var rule in style.Rules)
            {
                if (!(rule is DependencyPropertyValueRule depProp) ||
                    depProp.Selector.Property.Name != nameof(IVisualElement.Template))
                    continue;

                if (depProp.Declaration.Value is IVisualTemplate visualTemplate)
                {
                    template = visualTemplate;
                    return true;
                }
            }

            template = default!;
            return false;
        }

        public async Task ApplyVisualStylesAsync(IVisualElement visual,
                                                 IAttributeDictionary attributeDictionary,
                                                 IVisualLineage visualLineage,
                                                 IViewInflater viewInflater,
                                                 IVisualBootstrapper visualBootstrapper)
        {
            var useStyle = await _styleProvider.GetStyleForVisualAsync(visual, attributeDictionary);
            if (useStyle == null)
                return;

            await EnsureStyleVisualTemplates(visual, useStyle, viewInflater, 
                visualLineage, visualBootstrapper);

            TrySetVisualClass(visual, attributeDictionary);

            var appliedStyle = BuildAppliedStyle(useStyle, visual, visualLineage, visualBootstrapper);

            if (appliedStyle != null)
            {
                TrySetVisualStyle(visual, appliedStyle);
                appliedStyle.Execute(false);
            }
        }

        public void ApplyVisualCoreStyles(IVisualElement visual,
                                          IVisualBootstrapper visualBootstrapper)
        {
            var useStyle = _styleProvider.GetCoreStyleForVisual(visual);
            if (useStyle == null)
                return;
            
            var visualLineage = new VisualLineage();
            var appliedStyle = BuildAppliedStyle(useStyle, visual, visualLineage, visualBootstrapper);

            if (appliedStyle != null)
            {
                TrySetVisualStyle(visual, appliedStyle);
                appliedStyle.Execute(false);
            }
        }

        public IVisualStyleProvider StyleProvider { get; }

        private AppliedStyleRule? BuildAppliedRule(AppliedStyle appliedStyle,
                                                   IStyleRule rule,
                                                   IVisualElement rootVisual,
                                                   IVisualLineage visualLineage,
                                                   IVisualBootstrapper visualBootstrapper)
        {
            var appliedRule = new AppliedStyleRule(rule);

            foreach (var appliesTo in GetSelectableVisuals(appliedStyle, appliedRule,
                rootVisual, rule.Selector, visualLineage))
            {
                var assignments = _declarationWorker.BuildStyleValueAssignments(
                    appliesTo, visualLineage, rule, visualBootstrapper);

                foreach (var valueAssignment in assignments)
                    appliedRule.Assignments.Add(valueAssignment);
            }

            if (appliedRule.Assignments.Count > 0)
                return appliedRule;

            return default;
        }

        private static String? GetClassName(IVisualElement visual,
                                            IVisualLineage visualLineage)
        {
            if (!String.IsNullOrEmpty(visual.Class))
                return visual.Class;

            foreach (var item in visualLineage)
                if (!String.IsNullOrEmpty(item.Class))
                    return item.Class;

            return default;
        }

        private static IEnumerable<IVisualElement> GetSelectableVisuals(AppliedStyle appliedStyle,
                                                                        AppliedStyleRule appliedRule,
                                                                        IVisualElement rootVisual,
                                                                        IStyleSelector selector,
                                                                        IVisualLineage visualLineage)
        {
            switch (selector)
            {
                case AndStyleSelector andy:
                    var selectables = GetSelectableVisualsImpl(appliedStyle, appliedRule,
                        rootVisual, andy, visualLineage, 0);

                    foreach (var selectable in selectables)
                        yield return selectable;

                    break;

                case VisualStateSelector stateSelector:
                    if (IsVisualSelectable(appliedStyle, appliedRule, rootVisual,
                        stateSelector, visualLineage))
                        yield return rootVisual;
                    break;

                default:
                    if (IsVisualSelectable(rootVisual, selector, visualLineage))
                        yield return rootVisual;
                    break;
            }
        }

        private static Boolean IsVisualSelectable(AppliedStyle appliedStyle,
                                                  AppliedStyleRule appliedRule,
                                                  IVisualElement currentVisual,
                                                  VisualStateSelector stateSelector,
                                                  IVisualLineage visualLineage)
        {
            if (!IsVisualSelectable(currentVisual,
                stateSelector.BaseSelector, visualLineage))
                return false;

            foreach (var stateType in GetStateTypes(stateSelector.StateType))
            {
                if (!TryGetStateFilterItems(stateType, currentVisual,
                    out var dependencyProperty, out var value))
                    continue;

                appliedStyle.MonitorPropertyChange(dependencyProperty, currentVisual);

                var condition = new AppliedStyleCondition(currentVisual, dependencyProperty,
                    value!.Value);

                appliedRule.Conditions.Add(condition);
            }

            return true;
        }

        private static IEnumerable<IVisualElement> GetSelectableVisualsImpl(AppliedStyle appliedStyle,
                                                                            AppliedStyleRule appliedRule,
                                                                            IVisualElement rootVisual,
                                                                            AndStyleSelector selectors,
                                                                            IVisualLineage visualLineage,
                                                                            Int32 selectorIndex)
        {
            var currentVisual = rootVisual;

            for (var c = selectorIndex; c < selectors.Count; c++)
            {
                var currentSelector = selectors[c];

                switch (currentSelector)
                {
                    case CombinatorSelector combinator:
                        switch (combinator.Combinator)
                        {
                            case Combinator.Invalid:
                            case Combinator.None:
                                throw new InvalidOperationException();

                            case Combinator.Descendant:
                                break;

                            case Combinator.Child:
                                if (!(currentVisual is IVisualContainer container))
                                    yield break;

                                foreach (var childVisual in container.Children.GetAllChildren())
                                {
                                    visualLineage.PushVisual(childVisual);

                                    var selectableChildren = GetSelectableVisualsImpl(appliedStyle,
                                        appliedRule, childVisual, selectors, visualLineage, c + 1);

                                    foreach (var selectable in selectableChildren)
                                        yield return selectable;

                                    visualLineage.AssertPopVisual(childVisual);
                                }

                                break;

                            case Combinator.GeneralSibling:
                                throw new NotImplementedException();

                            case Combinator.AdjacentSibling:

                                var nextSibling = visualLineage.GetNextSibling();

                                if (nextSibling != null)
                                {
                                    visualLineage.PushVisual(nextSibling);

                                    var selectableSiblings = GetSelectableVisualsImpl(appliedStyle,
                                        appliedRule,
                                        nextSibling, selectors, visualLineage, c + 1);

                                    foreach (var selectable in selectableSiblings)
                                        yield return selectable;

                                    visualLineage.AssertPopVisual(nextSibling);
                                }

                                break;
                            case Combinator.Column:
                                throw new NotImplementedException();

                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;

                    case VisualStateSelector stateSelector:

                        if (!IsVisualSelectable(appliedStyle, appliedRule, currentVisual,
                                stateSelector, visualLineage))
                            yield break;

                        goto default;

                    default:
                        if (!IsVisualSelectable(currentVisual, currentSelector, visualLineage))
                            yield break;

                        if (c == selectors.Count - 1)
                            yield return currentVisual;
                        break;
                }
            }
        }


        private static IEnumerable<VisualStateType> GetStateTypes(VisualStateType baseStates)
        {
            if ((baseStates & VisualStateType.Active) == VisualStateType.Active)
                yield return VisualStateType.Active;

            if ((baseStates & VisualStateType.Checked) == VisualStateType.Checked)
                yield return VisualStateType.Checked;

            if ((baseStates & VisualStateType.Disabled) == VisualStateType.Disabled)
                yield return VisualStateType.Disabled;

            if ((baseStates & VisualStateType.Focus) == VisualStateType.Focus)
                yield return VisualStateType.Focus;

            if ((baseStates & VisualStateType.Hover) == VisualStateType.Hover)
                yield return VisualStateType.Hover;
        }

        private static Boolean IsVisualSelectable(IVisualElement visual,
                                                  IStyleSelector selector,
                                                  IVisualLineage visualLineage)
        {
            switch (selector)
            {
                case AndStyleSelector andy:
                    foreach (var sel in andy.Selectors)
                        if (!IsVisualSelectable(visual, sel, visualLineage))
                            return false;

                    return true;

                case VisualTypeStyleSelector typeSelector:
                    var res = typeSelector.VisualType.IsInstanceOfType(visual);
                    return res;

                case DependencyPropertySelector _:
                    return true;

                case ClassStyleSelector classSelector:

                    var className = GetClassName(visual, visualLineage);

                    return className == classSelector.ClassName;

                case ContentAppenderSelector contentAppender:
                    var res2 = IsVisualSelectable(visual, 
                        contentAppender.TypeSelector, visualLineage);

                    return res2;

                case VisualStateSelector stateSelector:
                    // the state selector isn't tied to a specific type and is combined
                    // with a type selector (presumably...)
                    return IsVisualSelectable(visual, stateSelector.BaseSelector, visualLineage);


                case AllStyleSelector _:
                    return true;

                default:
                    throw new NotImplementedException();
            }
        }

        private static Boolean TryGetStateFilterItems(VisualStateType stateType,
                                                      IVisualElement visual,
                                                      out IDependencyProperty dependencyProperty,
                                                      out Boolean? value)
        {
            switch (stateType)
            {
                case VisualStateType.Invalid:
                    break;
                case VisualStateType.None:
                    break;

                case VisualStateType.Hover:
                    if (!(visual is IInteractiveVisual))
                        goto fail;

                    dependencyProperty = InteractiveVisualProperties.IsMouseOverProperty;
                    value = true;
                    return true;

                case VisualStateType.Active:
                    if (!(visual is IInteractiveVisual))
                        goto fail;

                    dependencyProperty = InteractiveVisualProperties.IsActiveProperty;
                    value = true;
                    return true;

                case VisualStateType.Checked:
                    if (!(visual is IToggleButton))
                        goto fail;

                    dependencyProperty = ToggleButton.IsCheckedProperty;
                    value = true;
                    return true;

                case VisualStateType.Disabled:

                    dependencyProperty = VisualElement.IsEnabledProperty;
                    value = false;
                    return true;

                case VisualStateType.Focus:
                    if (!(visual is IInteractiveVisual))
                        goto fail;

                    dependencyProperty = InteractiveVisualProperties.IsFocusedProperty;
                    value = true;


                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            fail:
            dependencyProperty = default!;
            value = default;
            return false;
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

        private void TrySetVisualStyle(IVisualElement visual,
                                       IAppliedStyle styleSheet)
        {
            var propAccessor = _propertyProvider.GetPropertyAccessor(visual.GetType(),
                nameof(IVisualElement.Style));
            if (propAccessor.CanWrite)
            {
                Object oVisual = visual;
                propAccessor.SetPropertyValue(ref oVisual, styleSheet);
            }
        }

        private readonly IDeclarationWorker _declarationWorker;
        private readonly IPropertyProvider _propertyProvider;
        private readonly IVisualStyleProvider _styleProvider;
    }
}
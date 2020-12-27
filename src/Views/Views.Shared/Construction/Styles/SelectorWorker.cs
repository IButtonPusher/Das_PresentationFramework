using System;
using System.Collections.Generic;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;
using Das.Views.Styles.Construction;
using Das.Views.Styles.Selectors;

namespace Das.Views.Construction
{
    public class SelectorWorker
    {
        /// <summary>
        /// Returns the root visual and/or any child visuals that meet the
        /// requirements of the selector
        /// </summary>
        public static IEnumerable<IVisualElement> GetSelectableVisuals(IVisualElement rootVisual,
                                                                       IStyleSelector selector,
                                                                       IVisualLineage visualLineage)
        {
            switch (selector)
            {
                case AndStyleSelector andy:
                    var selectables = GetSelectableVisualsImpl(rootVisual, andy, visualLineage, 0);

                    foreach (var selectable in selectables)
                        yield return selectable;

                    break;

                default:
                    if (IsVisualSelectable(rootVisual, selector, visualLineage))
                        yield return rootVisual;
                    break;
            }
        }

        private static IEnumerable<IVisualElement> GetSelectableVisualsImpl(IVisualElement rootVisual,
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

                                    var selectableChildren = GetSelectableVisualsImpl(childVisual, 
                                        selectors, visualLineage, c + 1);

                                    foreach (var selectable in selectableChildren)
                                        yield return selectable;

                                    visualLineage.AssertPopVisual(childVisual);
                                }

                                break;
                            
                            
                            case Combinator.GeneralSibling:
                                break;
                            case Combinator.AdjacentSibling:

                                var nextSibling = visualLineage.GetNextSibling();

                                if (nextSibling != null)
                                {
                                    visualLineage.PushVisual(nextSibling);

                                    var selectableSiblings = GetSelectableVisualsImpl(nextSibling,
                                        selectors, visualLineage, c + 1);

                                    foreach (var selectable in selectableSiblings)
                                        yield return selectable;

                                    visualLineage.AssertPopVisual(nextSibling);
                                }

                                break;
                            case Combinator.Column:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;

                    //case VisualStateSelector stateSelector:
                    //    if (IsVisualSelectable(currentVisual, stateSelector.BaseSelector, visualLineage))
                    //    {}
                    //    break;

                    default:
                        if (!IsVisualSelectable(currentVisual, currentSelector, visualLineage))
                            yield break;

                        if (c == selectors.Count - 1)
                            yield return currentVisual;
                        break;
                }

            }
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
                    return false;

                case ClassStyleSelector classSelector:

                    var className = GetClassName(visual, visualLineage);

                    return className == classSelector.ClassName;

                case ContentAppenderSelector contentAppender:
                    var res2 = IsVisualSelectable(visual, contentAppender.TypeSelector, visualLineage);
                    if (res2)
                    {}
                    return res2;

                case VisualStateSelector stateSelector:
                    // the state selector isn't tied to a specific type and is combined
                    // with a type selector (presumably...)
                    return IsVisualSelectable(visual, stateSelector.BaseSelector, visualLineage);


                default:
                    throw new NotImplementedException();
            }
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
    }
}

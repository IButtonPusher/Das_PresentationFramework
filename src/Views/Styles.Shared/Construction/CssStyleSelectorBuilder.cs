using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Styles;
using Das.Views.Styles.Construction;
using Das.Views.Styles.Selectors;

namespace Das.Views.Construction.Styles
{
    public class CssStyleSelectorBuilder : IStyleSelectorBuilder
    {
        public CssStyleSelectorBuilder(IVisualAliasProvider visualAliasProvider)
        {
            _visualAliasProvider = visualAliasProvider;
        }

        public IStyleSelector GetSelector(IMarkupNode cssNode)
        {
            GetSelectorParts(cssNode, out var selectors, out var combinator);

            switch (selectors.Count)
            {
                case 0:
                    throw new InvalidOperationException();

                case 1:
                    return selectors[0];

                default:
                    return new AndStyleSelector(selectors);
            }

            //if (selectors.Count == 1)
            //    return selectors[0];

            

            //return GetSelectorFromParts(selectors, combinator);
        }

        private void GetSelectorParts(IMarkupNode cssNode,
                                      out List<IStyleSelector> currentAndSelectors,
                                      out List<Combinator> combinators)
        {
            currentAndSelectors = new List<IStyleSelector>();
            combinators = new List<Combinator>();

            var name = cssNode.Name;

            if (name == "*")
            {
                currentAndSelectors.Add(AllStyleSelector.Instance);
                return;
            }

            var tokens = name.Split();

            for (var c = 0; c < tokens.Length; c++)
            {
                var currentToken = tokens[c];
                if (currentToken.Length == 0)
                    continue;

                switch (currentToken[0])
                {
                    case '.':
                    {
                        var classSelector = GetClassStyleSelector(currentToken);

                        currentAndSelectors.Add(classSelector);
                        break;
                    }

                    case '>':
                        currentAndSelectors.Add(new CombinatorSelector(Combinator.Child));
                        //combinators.Add(Combinator.Child);
                        break;

                    case '+':
                        currentAndSelectors.Add(new CombinatorSelector(Combinator.AdjacentSibling));
                        //combinators.Add(Combinator.AdjacentSibling);
                        break;

                    case '~':
                        currentAndSelectors.Add(new CombinatorSelector(Combinator.AdjacentSibling));
                        //combinators.Add(Combinator.GeneralSibling);
                        break;

                    case '|':
                        if (currentToken[1] == '|')
                        {
                            currentAndSelectors.Add(new CombinatorSelector(Combinator.Column));
                            //combinators.Add(Combinator.Column);
                        }

                        break;

                    case ' ':
                        currentAndSelectors.Add(new CombinatorSelector(Combinator.Descendant));
                        break;

                    default:

                        var colonIndex = currentToken.IndexOf(':');

                        if (colonIndex > 0)
                        {
                            var visualName = currentToken.Substring(0, colonIndex);
                            var typeSelector = GetVisualTypeSelector(visualName);

                            if (currentToken.Length > colonIndex + 1 &&
                                currentToken[colonIndex + 1] == ':')
                            {
                                // span::before

                                var appenderName = currentToken.Substring(colonIndex + 2);
                                var appendSelector = GetContentAppender(typeSelector, appenderName);
                                currentAndSelectors.Add(appendSelector);

                                break;
                            }

                            // PSEUDO
                            // input:checked
                            //var stateName = currentToken.Substring(colonIndex + 1);
                            var stateType = GetStateType(currentToken, colonIndex);

                            var visualStateSelector = new VisualStateSelector(typeSelector, stateType);
                            //return visualStateSelector;

                            currentAndSelectors.Add(visualStateSelector);

                            //var stateSelector = GetVisualStateSelector(stateName);
                            //var typeAndStateSelector = new AndStyleSelector(typeSelector, stateSelector);
                            //currentAndSelectors.Add(typeAndStateSelector);
                            //currentAndSelectors.Add(stateSelector);
                        }
                        else
                        {
                            var visualTypeSelector = GetVisualTypeSelector(currentToken);
                            currentAndSelectors.Add(visualTypeSelector);
                        }

                        break;
                }
            }
        }

        private IStyleSelector GetSelectorFromParts(List<IStyleSelector> selectors,
                                                List<Combinator> combinators)
        {
            if (selectors.Count == 1)
                return selectors[0];

            var resultSelector = selectors[0];
            selectors.RemoveAt(0);

            foreach (var combinator in combinators)
            {
                switch (combinator)
                {
                    case Combinator.Child:
                        resultSelector = new ChildSelector(
                            resultSelector, selectors[0]);
                        selectors.RemoveAt(0);
                        
                        break;

                    case Combinator.AdjacentSibling:
                        resultSelector = new AdjacentSiblingSelector(
                            resultSelector, selectors[0]);
                        selectors.RemoveAt(0);
                        

                        break;
                }
            }

            if (selectors.Count > 0)
                throw new InvalidOperationException();

            return resultSelector;
           
        }

        private static IStyleSelector ThrowForWrongCount(Int32 needed,
                                                         Combinator combinator)
        {
            throw new InvalidOperationException(needed + " selectors are required for combinator: " 
                                                       + combinator);
        }

        private static IStyleSelector GetClassStyleSelector(String currentToken)
        {
            var colonIndex = currentToken.IndexOf(':');
            if (colonIndex == -1)
                return new ClassStyleSelector(currentToken);

            var andSelectors = new List<IStyleSelector>();

            var className = currentToken.Substring(0, colonIndex);
            var classSelector = new ClassStyleSelector(className);
            //andSelectors.Add(new ClassStyleSelector(className));

            var stateType = GetStateType(currentToken, colonIndex);

            var visualStateSelector = new VisualStateSelector(classSelector, stateType);
                return visualStateSelector;
                //andSelectors.Add(GetVisualStateSelector(stateName));

                //return new AndStyleSelector(andSelectors);
        }

        private static VisualStateType GetStateType(String currentToken,
                                                    Int32 colonIndex)
        {
            var result = VisualStateType.Invalid;

            while (colonIndex > -1)
            {
                String stateName;

                var nextColonIndex = currentToken.IndexOf(':', colonIndex + 1);
                if (nextColonIndex > 0)
                {
                    stateName = currentToken.Substring(colonIndex + 1, nextColonIndex - colonIndex - 1);
                }
                else
                {
                    stateName = currentToken.Substring(colonIndex + 1);
                }

                if (!Enum.TryParse<VisualStateType>(stateName, true, out var stateType))
                    throw new NotImplementedException();

                result |= stateType;

                colonIndex = nextColonIndex;
            }

            return result;
        }

        //private static IStyleSelector GetVisualStateSelector(String stateText)
        //{
        //    var colonIndex = stateText.IndexOf(':');
        //    if (colonIndex == -1)
        //        return GetVisualStateSelectorImpl(stateText);

        //    var tokens = stateText.Split(':');

        //    return new AndStyleSelector(tokens.Select(GetVisualStateSelectorImpl));
        //}

        //private static VisualStateSelector GetVisualStateSelectorImpl(String stateText)
        //{
        //    if (!Enum.TryParse<VisualStateType>(stateText, true, out var stateType))
        //        throw new NotImplementedException();

        //    return new VisualStateSelector(stateType);
        //}

        private IStyleSelector GetVisualTypeSelector(String currentToken)
        {
            var visualType = GetVisualType(currentToken);

            return new VisualTypeStyleSelector(visualType);
        }

        private Type GetVisualType(String currentToken)
        {
            return _visualAliasProvider.GetVisualTypeFromAlias(currentToken) ?? 
                   throw new NotImplementedException();
        }

        private static IStyleSelector GetContentAppender(IStyleSelector typeSelector,
                                                         String appendTypeName)
        {
            if (!Enum.TryParse<ContentAppendType>(appendTypeName, true, out var appendType))
                throw new InvalidOperationException();

            return new ContentAppenderSelector(typeSelector, appendType);
        }

        private readonly IVisualAliasProvider _visualAliasProvider;
    }
}
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
            GetSelectorParts(cssNode, out var selectors, out _);

            switch (selectors.Count)
            {
                case 0:
                    throw new InvalidOperationException();

                case 1:
                    return selectors[0];

                default:
                    return new AndStyleSelector(selectors);
            }

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
                        break;

                    case '+':
                        currentAndSelectors.Add(new CombinatorSelector(Combinator.AdjacentSibling));
                        break;

                    case '~':
                        currentAndSelectors.Add(new CombinatorSelector(Combinator.AdjacentSibling));
                        break;

                    case '|':
                        if (currentToken[1] == '|')
                        {
                            currentAndSelectors.Add(new CombinatorSelector(Combinator.Column));
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
                            var stateType = GetStateType(currentToken, colonIndex);

                            var visualStateSelector = new VisualStateSelector(typeSelector, stateType);
                            

                            currentAndSelectors.Add(visualStateSelector);
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


        private static IStyleSelector GetClassStyleSelector(String currentToken)
        {
            var colonIndex = currentToken.IndexOf(':');
            if (colonIndex == -1)
                return new ClassStyleSelector(currentToken);

            var className = currentToken.Substring(0, colonIndex);
            var classSelector = new ClassStyleSelector(className);
            

            var stateType = GetStateType(currentToken, colonIndex);

            var visualStateSelector = new VisualStateSelector(classSelector, stateType);
                return visualStateSelector;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Styles;
using Das.Views.Styles.Selectors;

namespace Das.Views.Construction.Styles
{
    public class StyleSelectorBuilder : IStyleSelectorBuilder
    {
        public StyleSelectorBuilder(IVisualAliasProvider visualAliasProvider)
        {
            _visualAliasProvider = visualAliasProvider;
        }

        public IStyleSelector GetSelector(IMarkupNode cssNode)
        {
            var currentAndSelectors = new List<IStyleSelector>();

            var name = cssNode.Name;

            if (name == "*")
                return AllStyleSelector.Instance;

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
                        break;

                    case '+':
                        break;

                    default:

                        var colonIndex = currentToken.IndexOf(':');

                        if (colonIndex > 0)
                        {
                            var visualName = currentToken.Substring(0, colonIndex);
                            var visualTypeSelector = GetVisualTypeSelector(visualName);
                            
                            if (currentToken.Length > colonIndex + 1 &&
                                currentToken[colonIndex + 1] == ':')
                            {
                                // span::before

                                var appenderName = currentToken.Substring(colonIndex + 2);
                                var appendSelector = GetContentAppender(visualTypeSelector, appenderName);
                                currentAndSelectors.Add(appendSelector);

                                break;
                            }
                            
                            // input:checked
                            var stateName = currentToken.Substring(colonIndex + 1);
                            currentAndSelectors.Add(visualTypeSelector);
                            
                            var stateSelector = GetVisualStateSelector(stateName);
                            currentAndSelectors.Add(stateSelector);
                        }
                        else
                        {
                            var visualTypeSelector = GetVisualTypeSelector(currentToken);
                            currentAndSelectors.Add(visualTypeSelector);
                        }

                        break;
                }
            }

            switch (currentAndSelectors.Count)
            {
                case 0:
                    throw new InvalidOperationException();        
                
                case 1:
                    return currentAndSelectors[0];
                
                default:
                    return new AndStyleSelector(currentAndSelectors);
            }
        }

        private static IStyleSelector GetClassStyleSelector(String currentToken)
        {
            var colonIndex = currentToken.IndexOf(':');
            if (colonIndex == -1)
                return new ClassStyleSelector(currentToken);

            var andSelectors = new List<IStyleSelector>();

            var className = currentToken.Substring(0, colonIndex);
            andSelectors.Add(new ClassStyleSelector(className));

            var stateName = currentToken.Substring(colonIndex + 1);
            andSelectors.Add(GetVisualStateSelector(stateName));

            return new AndStyleSelector(andSelectors);
        }

        private static IStyleSelector GetVisualStateSelector(String stateText)
        {
            var colonIndex = stateText.IndexOf(':');
            if (colonIndex == -1)
                return GetVisualStateSelectorImpl(stateText);

            var tokens = stateText.Split(':');

            return new AndStyleSelector(tokens.Select(GetVisualStateSelectorImpl));
        }

        private static VisualStateSelector GetVisualStateSelectorImpl(String stateText)
        {
            if (!Enum.TryParse<VisualStateType>(stateText, true, out var stateType))
                throw new NotImplementedException();

            return new VisualStateSelector(stateType);
        }

        private IStyleSelector GetVisualTypeSelector(String currentToken)
        {
            var visualType = _visualAliasProvider.GetVisualTypeFromAlias(currentToken);
            if (visualType == null)
                throw new NotImplementedException();

            return new VisualTypeStyleSelector(visualType);
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
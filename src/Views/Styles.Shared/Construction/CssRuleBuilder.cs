using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Styles;
using Das.Views.Styles.Declarations;
using Das.Views.Styles.Declarations.Transform;
using Das.Views.Styles.Declarations.Transition;

namespace Das.Views.Construction.Styles
{
    public class CssRuleBuilder : ICssRuleBuilder
    {
        public CssRuleBuilder(IStyleSelectorBuilder selectorBuilder,
                                IStyleVariableAccessor variableAccessor)
        {
            _selectorBuilder = selectorBuilder;
            _variableAccessor = variableAccessor;
        }

        static CssRuleBuilder()
        {
            _vendorPrefixes = new HashSet<String>(StringComparer.OrdinalIgnoreCase) {"-moz-", "-webkit-"};
        }

        public IStyleRule? GetRule(IMarkupNode cssNode)
        {
            var selector = _selectorBuilder.GetSelector(cssNode);
            var declarations = GetDeclarations(cssNode);
            var res = new StyleValueRule(selector, declarations);

            if (res.Declarations.Length == 0)
                return default;

            return res;
        }

        public IEnumerable<IStyleRule> GetRules(String css)
        {
            var nodes = CssNodeBuilder.GetMarkupNodes(css).ToArray();

            foreach (var node in nodes)
            {
                var rule = GetRule(node);
                if (rule != null)
                    yield return rule;
            }
        }

        private IStyleDeclaration? GetDeclaration(DeclarationProperty property,
                                                  String value)
        {
            switch (property)
            {
                case DeclarationProperty.ZIndex:
                    return new ScalarDeclaration<Int32>(value, _variableAccessor, property);

                case DeclarationProperty.Position:
                    return new PositionDeclaration(value, _variableAccessor);

                case DeclarationProperty.Display:
                    return new DisplayDeclaration(value, _variableAccessor);

                case DeclarationProperty.Color:
                case DeclarationProperty.BackgroundColor:
                    return new ColorDeclaration(value, property, _variableAccessor);

                case DeclarationProperty.FontFamily:
                    return new FontFamilyDeclaration(value, _variableAccessor);

                case DeclarationProperty.FontSize:
                    return new FontSizeDeclaration(value, _variableAccessor);

                case DeclarationProperty.LineHeight:
                    return new LineHeightDeclaration(value, _variableAccessor);

                case DeclarationProperty.Appearance:
                    return new AppearanceDeclaration(value, _variableAccessor);

                case DeclarationProperty.Right:
                case DeclarationProperty.Left:
                case DeclarationProperty.Top:
                case DeclarationProperty.Bottom:
                    return new QuantityDeclaration(value, _variableAccessor, property);

                case DeclarationProperty.Margin:
                    return new MarginDeclaration(value, _variableAccessor);

                case DeclarationProperty.BorderRadius:
                    return new BorderRadiusDeclaration(value, _variableAccessor);

                case DeclarationProperty.Width:
                case DeclarationProperty.Height:
                    return new QuantityDeclaration(value, _variableAccessor, property);

                
                case DeclarationProperty.Outline:
                    return new OutlineDeclaration(value, _variableAccessor);
                
                case DeclarationProperty.Opacity:
                    return new ScalarDeclaration<Double>(value, _variableAccessor, property);
                    //return new DoubleDeclaration(value, _variableAccessor, property);
                
                case DeclarationProperty.Transform:
                    return new TransformDeclaration(value, _variableAccessor);
                
                case DeclarationProperty.PointerEvents:
                    return new EnumDeclaration<PointerEventType>(value, PointerEventType.Auto,
                        _variableAccessor, property);
                
                case DeclarationProperty.Transition:
                    return new MultiTransitionDeclaration(value, _variableAccessor);
                
                case DeclarationProperty.Cursor:
                    return new EnumDeclaration<CursorType>(value, CursorType.Pointer,
                        _variableAccessor, property);
                
                case DeclarationProperty.Content:
                    return new StringDeclaration(value, _variableAccessor, property);
                
                case DeclarationProperty.Float:
                    return new EnumDeclaration<FloatType>(value, FloatType.None,
                        _variableAccessor, property);
                
                case DeclarationProperty.VerticalAlign:
                    return new VerticalAlignDeclaration(value, _variableAccessor);
                
                case DeclarationProperty.BoxShadow:
                    return new BoxShadowDeclaration(value, _variableAccessor);
                
                default:
                    throw new NotImplementedException();
            }
        }

        private IEnumerable<IStyleDeclaration> GetDeclarations(IMarkupNode cssNode)
        {
            foreach (var attribute in cssNode.GetAllAttributes())
            {
                if (IsVendorProperty(attribute.Key))
                    continue;

                var propertyName = attribute.Key.IndexOf("-", StringComparison.OrdinalIgnoreCase) > 0
                    ? attribute.Key.Replace("-", "")
                    : attribute.Key;

                if (!Enum.TryParse<DeclarationProperty>(propertyName, true, out var declarationProperty))
                {
                    throw new NotImplementedException();
                }


                var declaration = GetDeclaration(declarationProperty, attribute.Value);
                if (declaration != null)
                    yield return declaration;
            }
        }

        private static Boolean IsVendorProperty(String property)
        {
            if (property.IndexOf('-') != 0) 
                return false;
            
            var endOfPrefix = property.IndexOf('-', 1);
            if (endOfPrefix > 0)
            {
                var vendorPrefix = property.Substring(0, endOfPrefix + 1);
                return _vendorPrefixes.Contains(vendorPrefix);
            }

            return false;
        }

        private readonly IStyleSelectorBuilder _selectorBuilder;
        private readonly IStyleVariableAccessor _variableAccessor;
        private static readonly HashSet<String> _vendorPrefixes;
    }
}
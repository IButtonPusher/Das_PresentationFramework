using System;
using Das.Views.Construction;
using Das.Views.Construction.Styles;
using Das.Views.Controls;
using Das.Views.Styles.Declarations;
using Das.Views.Styles.Selectors;
using Das.Views.Templates;

namespace Das.Views.Styles.Construction
{
    public class XmlStyleRuleBuilder : IXmlStyleRuleBuilder
    {
        public IStyleRule? GetRule(IMarkupNode markupNode, 
                                   Type targetType)
        {
            if (markupNode.TryGetAttributeValue(nameof(DependencyPropertySelector.Property),
                out var propertyName))
            {
                if (DependencyProperty.TryGetDependecyProperty(targetType, propertyName, out var depProp))
                {
                    var selector = new DependencyPropertySelector(depProp);

                    var propVal = BuildPropertyValue(depProp, markupNode);
                    
                    var objDeclaration = new ObjectDeclaration(propVal);

                    return new DependencyPropertyValueRule(selector, objDeclaration);
                }

            }

            throw new NotImplementedException();
        }

        private static Object? BuildPropertyValue(IDependencyProperty property,
                                                  IMarkupNode parentNode)
        {
            if (typeof(IVisualTemplate).IsAssignableFrom(property.PropertyType))
            {
                if (parentNode.ChildrenCount != 1)
                    throw new NotImplementedException();
                
                return new DeferredVisualTemplate(parentNode[0]);
            }

            throw new NotImplementedException();
        }
    }
}

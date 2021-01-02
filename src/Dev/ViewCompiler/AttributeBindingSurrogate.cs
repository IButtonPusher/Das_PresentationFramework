using System;
using Das.Serializer;
using Das.Serializer.Scanners;
using Das.Views.DataBinding;

namespace Das.Views.DevKit
{
    public class AttributeBindingSurrogate : IAttributeValueSurrogates
    {
        private readonly IPropertyProvider _propertyProvider;

        public AttributeBindingSurrogate(IPropertyProvider propertyProvider)
        {
            _propertyProvider = propertyProvider;
        }

        public Boolean TryGetValue(ITextNode node, 
                                   String attributeName,
                                   String attributeText, 
                                   out Object value)
        {
            if (attributeName != "Binding" || 
                node.Value == null || 
                node.Type == null)
            {
                goto fail;
            }

            var valType = node.Value.GetType();
            var bindingProp = valType.GetProperty("Binding");
            if (bindingProp == null)
                goto fail;

            var gargs = bindingProp.PropertyType.GenericTypeArguments;

            if (gargs.Length == 0)
            {
                var propBinding = _propertyProvider.GetPropertyAccessor(node.Type, "Binding");

                value = new DeferredPropertyBinding(attributeText, "Binding", propBinding, null);
                return true;
            }

            if (gargs.Length == 1)
            {
                var genericDeferred = typeof(DeferredPropertyNameBinding<>).MakeGenericType(gargs[0]);
                var ctor = genericDeferred.GetConstructor(new[] { typeof(String)});
                if (ctor == null)
                    goto fail;

                value = ctor.Invoke(new Object[] {attributeText});
                return true;
            }

            fail:
            value = default!;
            return false;
        }
    }
}

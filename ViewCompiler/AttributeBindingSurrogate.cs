using System;
using Das.Serializer;
using Das.Serializer.Scanners;
using Das.Views.DataBinding;

namespace Das.Views.DevKit
{
    public class AttributeBindingSurrogate : IAttributeValueSurrogates
    {
        public Boolean TryGetValue(ITextNode node, 
                                   String attributeName,
                                   String attributeText, 
                                   out Object value)
        {
            if (attributeName != "Binding" || 
                node.Value == null)
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
                value = new DeferredPropertyBinding(attributeText);
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

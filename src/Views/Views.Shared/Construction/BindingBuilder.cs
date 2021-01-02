using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Das.Serializer;
using Das.ViewModels.Collections;
using Das.Views.DataBinding;

namespace Das.Views.Construction
{
    public class BindingBuilder : IBindingBuilder
    {
        public BindingBuilder(ITypeInferrer typeInferrer,
                              IPropertyProvider propertyProvider)
        {
            _typeInferrer = typeInferrer;
            _propertyProvider = propertyProvider;
            _cachedPropertyAccessors = new DoubleConcurrentDictionary<Type, String, IPropertyAccessor>();
        }

        public Dictionary<String, IDataBinding> GetBindingsDictionary(IMarkupNode node,
                                                                      Type? dataContextType,
                                                                      Dictionary<String, String>
                                                                          nameSpaceAssemblySearch)
        {
            Dictionary<String, IDataBinding> bindings;

            if (dataContextType != null)
            {
                bindings = new Dictionary<String, IDataBinding>();
                foreach (var kvp in GetBindings(dataContextType, node, nameSpaceAssemblySearch))
                {
                    bindings.Add(kvp.Key, kvp.Value);
                }
            }
            else bindings = _emptyBindings;

            return bindings;
        }

        public Type? InferDataContextTypeFromBindings(IEnumerable<IDataBinding> bindings,
                                                      Type? currentGenericArg)
        {
            //Type? genericChild;

            //if (currentGenericArg != null)
            //{
            //    var childArgs = currentGenericArg?.GetGenericArguments();

            //    if (childArgs != null && childArgs.Length > 0)
            //    {
            //        if (childArgs.Length == 1)
            //        {
            //            //genericChild = childArgs[0];
            //        }
            //        else throw new NotImplementedException();
            //    }
            //}

            foreach (var binding in bindings)
            {
                switch (binding)
                {
                    case DeferredPropertyBinding deferredPropertyBinding:

                        switch (deferredPropertyBinding.TargetPropertyName)
                        {
                            case nameof(IItemsControl.ItemsSource) when currentGenericArg != null:
                                //var sourceProp = currentGenericArg.GetProperty(
                                //    deferredPropertyBinding.SourcePropertyName);

                                var sourceProp = BaseBinding.GetBindingProperty(currentGenericArg,
                                    deferredPropertyBinding.SourcePropertyName);


                                if (!(sourceProp?.PropertyType is { } sourcePropType) ||
                                    !typeof(IEnumerable).IsAssignableFrom(sourcePropType) ||
                                    !sourcePropType.IsGenericType)
                                    continue;

                                var srcPropGenerics = sourcePropType.GetGenericArguments();
                                if (srcPropGenerics == null || srcPropGenerics.Length == 0)
                                    continue;

                                if (srcPropGenerics.Length != 1)
                                    throw new NotImplementedException();

                                return srcPropGenerics[0];

                            case nameof(IBindableElement.DataContext):

                                if (currentGenericArg == null)
                                    break;

                                var dcProp = BaseBinding.GetBindingProperty(currentGenericArg,
                                    deferredPropertyBinding.SourcePropertyName);

                                //var dcProp = currentGenericArg?.GetProperty(
                                //    deferredPropertyBinding.SourcePropertyName);

                                if (dcProp is { } validProp)
                                    return validProp.PropertyType;

                                break;
                        }

                        break;
                }
            }

            return currentGenericArg;
        }

        //private static PropertyInfo? GetProperty(Type declaringType,
        //                                         String propName)
        //{
        //    if (!propName.Contains(".")) 
        //        return declaringType.GetProperty(propName);

        //    var subPropTokens = propName.Split('.');
        //    var propInfo = declaringType.GetProperty(subPropTokens[0]);
        //    if (propInfo == null)
        //        return null;

        //    for (var c = 1; c < subPropTokens.Length; c++)
        //    {
        //        propInfo = BaseBinding.GetTypePropertyOrDie(propInfo.PropertyType, subPropTokens[c]);
        //        if (propInfo == null)
        //            return null;
        //    }

        //    return propInfo;
        //}

        private IEnumerable<KeyValuePair<String, IDataBinding>> GetBindings(Type dataContextType,
                                                                            IMarkupNode node,
                                                                            Dictionary<String, String>
                                                                                nameSpaceAssemblySearch)
        {
            foreach (var kvp in node.GetAllAttributes())
            {
                var valTrim = kvp.Value.Trim();
                if (valTrim.Length < 3 ||
                    valTrim[0] != '{' || valTrim[valTrim.Length - 1] != '}')
                    continue;

                var valExpression = valTrim.Substring(1, valTrim.Length - 2);

                var expressionGroups = valExpression.Split(',');

                var expressionTokens = expressionGroups[0].Split();

                String propName;

                switch (expressionTokens.Length)
                {
                    case 2 when expressionTokens[0] == "Binding":
                        propName = expressionTokens[1];
                        break;

                    case 1:
                        propName = expressionTokens[0];
                        break;

                    default:
                        throw new NotImplementedException();
                }


                IValueConverter? converter = null;


                if (expressionGroups.Length > 1)
                    for (var c = 1; c < expressionGroups.Length; c++)
                    {
                        var groupTokens = expressionGroups[c].Split('=');

                        if (groupTokens.Length != 2)
                            throw new NotImplementedException();

                        if (groupTokens[0].Trim() == "Converter")
                        {
                            var coverterTypeName = groupTokens[1];
                            var converterTokens = coverterTypeName.Split('.');

                            if (converterTokens.Length == 2)
                            {
                                var converterType = GetType(converterTokens[0], null, nameSpaceAssemblySearch);
                                var staticProp = converterType.GetProperty(converterTokens[1], BindingFlags.Static |
                                    BindingFlags.Public);

                                converter = staticProp?.GetValue(null, null) as IValueConverter;
                            }
                        }
                        else
                            throw new NotImplementedException();
                    }

                if (propName == ".")
                {
                    var dcBinding = new DataContextBinding(kvp.Key, converter);
                    yield return new KeyValuePair<String, IDataBinding>(kvp.Key, dcBinding);
                    continue;
                }

                if (BaseBinding.GetBindingProperty(dataContextType, propName) == null)
                    throw new MissingMemberException(dataContextType.Name, propName);

                var propAccessor = _cachedPropertyAccessors.GetOrAdd(dataContextType, propName, (d,
                        p) =>
                    _propertyProvider.GetPropertyAccessor(d, p));

                //var propAccessor = _propertyProvider.GetPropertyAccessor(dataContextType, propName);

                var binding = new DeferredPropertyBinding(propName, kvp.Key, propAccessor, converter);
                yield return new KeyValuePair<String, IDataBinding>(kvp.Key, binding);
            }
        }

        private Type GetType(String name,
                             String? genericArgName,
                             Dictionary<String, String> nameSpaceAssemblySearch)
        {
            if (!String.IsNullOrEmpty(genericArgName))
            {
                var letsTry = name + "[" + genericArgName + "]";
                var found = _typeInferrer.GetTypeFromClearName(letsTry, nameSpaceAssemblySearch, true);
                if (found != null)
                    return found;
            }

            return _typeInferrer.GetTypeFromClearName(name, nameSpaceAssemblySearch)
                   ?? throw new TypeLoadException(name);
        }

        private static readonly Dictionary<String, IDataBinding> _emptyBindings =
            new Dictionary<String, IDataBinding>();

        private readonly DoubleConcurrentDictionary<Type, String, IPropertyAccessor> _cachedPropertyAccessors;
        private readonly IPropertyProvider _propertyProvider;

        private readonly ITypeInferrer _typeInferrer;
    }
}

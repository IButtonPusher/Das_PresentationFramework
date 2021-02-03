using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Das.Serializer;
using Das.ViewModels.Collections;
using Das.Views.DataBinding;
using Das.Views.Resources;

namespace Das.Views.Construction
{
    public class BindingBuilder : IBindingBuilder
    {
        public BindingBuilder(ITypeInferrer typeInferrer,
                              IPropertyProvider propertyProvider,
                              IAssemblyList assemblies,
                              IResourceBuilder resourceBuilder)
        {
            _typeInferrer = typeInferrer;
            _propertyProvider = propertyProvider;
            _assemblies = assemblies;
            _resourceBuilder = resourceBuilder;
            _cachedPropertyAccessors = new DoubleConcurrentDictionary<Type, String, IPropertyAccessor>();
        }

        public async Task<Dictionary<String, IPropertyBinding>> GetBindingsDictionaryAsync(
            IMarkupNode node,
            Type? dataContextType,
            Dictionary<String, String>
                nameSpaceAssemblySearch)
        {
            Dictionary<String, IPropertyBinding> bindings;

            if (dataContextType != null)
            {
                bindings = new Dictionary<String, IPropertyBinding>();
                await foreach (var kvp in GetBindings(dataContextType, node, nameSpaceAssemblySearch))
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


        private async IAsyncEnumerable<KeyValuePair<String, IPropertyBinding>> GetBindings(
            Type dataContextType,
            IMarkupNode node,
            Dictionary<String, String>
                nameSpaceAssemblySearch)
        {
            foreach (var kvp in node.GetAllAttributes())
            {
                var bindingType = BindingType.None;

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
                    case 2 when expressionTokens[0] == DATA_BINDING:
                        propName = expressionTokens[1];
                        bindingType = BindingType.Data;
                        break;

                    case 2 when expressionTokens[0] == RESOURCE_BINDING:
                        propName = expressionTokens[1];
                        bindingType = BindingType.Resource;
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

                        if (groupTokens[0].Trim() == CONVERTER)
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
                    yield return new KeyValuePair<String, IPropertyBinding>(kvp.Key, dcBinding);
                    continue;
                }

                switch (bindingType)
                {
                    case BindingType.None:
                        continue;

                    case BindingType.Data:
                        if (BaseBinding.GetBindingProperty(dataContextType, propName) == null)
                            throw new MissingMemberException(dataContextType.Name, propName);

                        var propAccessor = _cachedPropertyAccessors.GetOrAdd(dataContextType, propName,
                            (d,
                             p) => _propertyProvider.GetPropertyAccessor(d, p));


                        var dataBinding = new DeferredPropertyBinding(propName, kvp.Key, propAccessor, converter);
                        yield return new KeyValuePair<String, IPropertyBinding>(kvp.Key, dataBinding);
                        break;

                    case BindingType.Resource:
                        var resourceBinding = await GetEmbeddedResourceBinding(propName);
                        if (resourceBinding != null)
                            yield return new(kvp.Key, resourceBinding);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private async Task<EmbeddedResourceBinding?> GetEmbeddedResourceBinding(String path)
        {
            var tokens = path.Split('.');
            if (tokens.Length < 3)
                return default;

            var asmName = tokens[0];

            for (var c = 1; c < tokens.Length - 2; c++)
                //if (_assemblies.TryGetAssembly(asmName + ".dll", out var foundAsm))
                if (_assemblies.TryGetAssemblyByName(asmName, out var foundAsm))
                {
                    var obj = await _resourceBuilder.GetEmbeddedResourceAsync(path, tokens, foundAsm);
                    if (obj == null)
                        return default;

                    return new EmbeddedResourceBinding(path, obj);
                }

            return default;
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

        private const String CONVERTER = "Converter";

        private const String DATA_BINDING = "Binding";
        private const String RESOURCE_BINDING = "Resource";

        private static readonly Dictionary<String, IPropertyBinding> _emptyBindings = new();
        private readonly IAssemblyList _assemblies;

        private readonly DoubleConcurrentDictionary<Type, String, IPropertyAccessor> _cachedPropertyAccessors;
        private readonly IPropertyProvider _propertyProvider;
        private readonly IResourceBuilder _resourceBuilder;

        private readonly ITypeInferrer _typeInferrer;
    }
}

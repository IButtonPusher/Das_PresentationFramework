using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Construction;
using Das.Views.Controls;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Templates;

namespace Das.Views
{
    public class ViewInflater : IViewInflater
    {
        public ViewInflater(IVisualBootstrapper visualBootstrapper,
                            IStringPrimitiveScanner xmlAttributeParser,
                            ITypeInferrer typeInferrer,
                            IBindingBuilder bindingBuilder,
                            IValueConverterProvider converterProvider,
                            IVisualTypeResolver visualTypeResolver)
        {
            _visualBootstrapper = visualBootstrapper;
            _typeInferrer = typeInferrer;
            _bindingBuilder = bindingBuilder;
            _converterProvider = converterProvider;
            _visualTypeResolver = visualTypeResolver;
            _attributeValueScanner = xmlAttributeParser;
        }

        static ViewInflater()
        {
            _defaultNamespaceSeed = new Dictionary<String, String>();
            _defaultNamespaceSeed["Das.Views.Panels"] = "Das.Views";
            _defaultNamespaceSeed["Das.Views.Controls"] = "Das.Views";
            _defaultNamespaceSeed["Das.Views.DataBinding"] = "Das.Views";
            _defaultNamespaceSeed["Das.Views.Templates"] = "Das.Views";
            _defaultNamespaceSeed["Das.Views"] = "Das.Views";
        }
        

        public TVisualElement InflateXml<TVisualElement>(String xml)
            where TVisualElement : IVisualElement
        {
            var visual = InflateXml(xml);
            return (TVisualElement) visual;
        }

        public TVisualElement InflateXml<TVisualElement>(String xml,
                                                         IDictionary<String, String> namespaceHints)
            where TVisualElement : IVisualElement
        {
            var visual = InflateXml(xml, namespaceHints);
            return (TVisualElement) visual;
        }

        public async Task<TVisualElement> InflateResourceXmlAsync<TVisualElement>(String resourceName)
            where TVisualElement : IVisualElement
        {
            var visual = await InflateResourceXmlAsync(resourceName);
            return (TVisualElement) visual;
        }

        public IVisualElement InflateXml(String xml)
        {
            return InflateXmlImpl(xml, _defaultNamespaceSeed);
        }

        public IVisualElement InflateXml(String xml,
                                         IDictionary<String, String> namespaceHints)
        {
            return InflateXmlImpl(xml, namespaceHints);
        }

        public async Task<IVisualElement> InflateResourceXmlAsync(String resourceName)
        {
            String xml;

            var thisExe = Assembly.GetExecutingAssembly();


            using (var stream = thisExe.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException(resourceName);

                using (var sr = new StreamReader(stream))
                {
                    xml = await sr.ReadToEndAsync();
                }
            }

            return InflateXml(xml);
        }

        private IDataTemplate BuildDataTemplate(IMarkupNode node,
                                                Type? dataContextType,
                                                Dictionary<String, String> nameSpaceAssemblySearch)
        {
            if (node.ChildrenCount != 1)
            {
                var visuals = new List<IVisualElement>();

                foreach (var childNode in node.Children)
                {
                    var visual = GetVisual(childNode, dataContextType, nameSpaceAssemblySearch);
                    visuals.Add(visual);
                }

                return new MultiDataTemplate(_visualBootstrapper, dataContextType, visuals);
            }

            var onlyChild = node.Children.First();

            switch (onlyChild.Name)
            {
                case nameof(MultiDataTemplate):
                    var bindings = _bindingBuilder.GetBindingsDictionary(node,
                        dataContextType, nameSpaceAssemblySearch);

                    dataContextType = _bindingBuilder.InferDataContextTypeFromBindings(bindings.Values,
                        dataContextType);
                    return BuildDataTemplate(onlyChild, dataContextType, nameSpaceAssemblySearch);

                case nameof(DataTemplate):
                    return BuildDataTemplate(onlyChild, dataContextType, nameSpaceAssemblySearch);

                default:
                    var visualContent = GetVisual(onlyChild, dataContextType, nameSpaceAssemblySearch);
                    return new DataTemplate(_visualBootstrapper, dataContextType, visualContent);
            }
        }


        //private static Dictionary<String, String> GetNamespaceAssemblySearch(
        //    IMarkupNode node,
        //    IDictionary<String, String> search)
        //{
        //    var nsAsmSearch = new Dictionary<String, String>(search);
        //    return GetNamespaceAssemblySearchImpl(node, nsAsmSearch);
        //}

        //private static Dictionary<String, String> GetNamespaceAssemblySearchImpl(IMarkupNode node,
        //    Dictionary<String, String> nsAsmSearch)
        //{
        //    foreach (var attribute in node.GetAllAttributes())
        //    {
        //        if (!attribute.Key.StartsWith("xmlns:"))
        //            continue;
        //        var tokens = attribute.Value.Split(';');


        //        String? asmName = null;
        //        String? ns = null;

        //        for (var t = 0; t < tokens.Length; t++)
        //        {
        //            var subTokens = tokens[t].Split(_namespaceSplitters);
        //            if (subTokens.Length != 2)
        //                continue;

        //            switch (subTokens[0])
        //            {
        //                case "clr-namespace":
        //                    ns = subTokens[1];
        //                    break;

        //                case "assembly":
        //                    asmName = subTokens[1];
        //                    break;
        //            }
        //        }

        //        if (ns == null)
        //            continue;

        //        if (asmName == null)
        //            continue;

        //        nsAsmSearch[ns] = asmName;
        //    }

        //    return nsAsmSearch;
        //}


        //private Type GetType(IMarkupNode node,
        //                     String? genericArgName,
        //                     Dictionary<String, String> nameSpaceAssemblySearch)
        //{
        //    var name = node.Name;

        //    var notGeneric = _typeInferrer.GetTypeFromClearName(name, nameSpaceAssemblySearch);
        //    if (notGeneric != null)
        //        return notGeneric;
            
        //    if (!String.IsNullOrEmpty(genericArgName) && node.ChildrenCount > 0)
        //    {
        //        var letsTry = name + "[" + genericArgName + "]";
        //        var found = _typeInferrer.GetTypeFromClearName(letsTry, nameSpaceAssemblySearch, true);
        //        if (found != null)
        //            return found;
        //    }

        //    throw new TypeLoadException(name);
        //}

        /// <summary>
        ///     Builds a visual from a markup node.  Infers the data context type and instantiates
        ///     a generic visual if possible
        /// </summary>
        /// <param name="node">an xml/json etc node</param>
        /// <param name="dataContextType">the data context type of the parent visual.</param>
        /// <param name="nameSpaceAssemblySearch"></param>
        /// <returns></returns>
        private IVisualElement GetVisual(IMarkupNode node,
                                         Type? dataContextType,
                                         Dictionary<String, String> nameSpaceAssemblySearch)
        {
            var bindings = _bindingBuilder.GetBindingsDictionary(node,
                dataContextType, nameSpaceAssemblySearch);


            dataContextType = _bindingBuilder.InferDataContextTypeFromBindings(bindings.Values,
                dataContextType);

            if (!node.TryGetAttributeValue("ContextType", out var currentGenericArgName))
                currentGenericArgName = dataContextType?.Name;
            else
                dataContextType = _typeInferrer.GetTypeFromClearName(currentGenericArgName,
                    nameSpaceAssemblySearch, true);

            var visualType = _visualTypeResolver.GetType(node, currentGenericArgName, 
                nameSpaceAssemblySearch);

            IVisualElement visual;

            if (typeof(IContentContainer).IsAssignableFrom(visualType))
            {
                visual = BuildContentVisual(node, dataContextType, 
                    nameSpaceAssemblySearch, visualType);
            }
            else if (visualType is { } validVisualType)
            {
                visual = _visualBootstrapper.Instantiate<IVisualElement>(validVisualType);

                if (node.ChildrenCount > 0) 
                    InflateChildNodes(node, visual, dataContextType, nameSpaceAssemblySearch);

                if (node.InnerText is {} innerText && 
                    innerText.Trim() is {} validInnerText && validInnerText.Length > 0 &&
                    GetAttribute<ContentPropertyAttribute>(validVisualType) is {} cp &&
                    _typeInferrer.FindPublicProperty(validVisualType, cp.Name) is {} contentProp && 
                    contentProp.PropertyType == typeof(String))
                {
                    // zb <Label>hello world</Label>
                    contentProp.SetValue(visual, validInnerText, null);
                }
                
            }
            else throw new NotImplementedException();

            if (bindings.Count > 0 && visual is IBindableElement bindable)
                foreach (var binding in bindings)
                    bindable.AddBinding(binding.Value);

            SetHardCodedValues(visual, node, bindings);

            return visual;
        }

        private static T GetAttribute<T>(Type type)
            where T : Attribute
        {
            var res = Attribute.GetCustomAttribute(type, typeof(T));
            switch (res)
            {
                case T good:
                    return good;
                
                default:
                    return default!;
            }
        }

        private void InflateChildNodes(IMarkupNode node,
                                       IVisualElement visual,
                                       Type? dataContextType,
                                       Dictionary<String, String> nameSpaceAssemblySearch)
        {
            foreach (var childNode in node.Children)
            {
                var visualChild = InflateChild(childNode, visual, dataContextType, 
                    nameSpaceAssemblySearch, out var childType, out var visualProp);

                if (childType == ChildNodeType.PropertyValue && visualProp is { } prop)
                {
                    prop.SetValue(visual, visualChild, null);
                }
                else if (visualChild is IVisualElement childVisual)
                {
                    AddChildVisual(visual, childVisual);
                }
            }
        }

        private IContentVisual BuildContentVisual(IMarkupNode node,
                                                  Type? dataContextType,
                                                  Dictionary<String, String> nameSpaceAssemblySearch,
                                                  Type visualType)
        {
            IVisualElement? contentVisual = null;

            var contentContainer = _visualBootstrapper.Instantiate<IContentVisual>(visualType);
                
            switch (node.ChildrenCount)
            {
                case 1:
                {
                    var currentNode = node[0];
                    var childObj = InflateChild(currentNode, contentContainer,
                        dataContextType, nameSpaceAssemblySearch,
                        out var childType,
                        out var childProp);

                    if (childType == ChildNodeType.ChildVisual && childObj is IVisualElement childVisual)
                        contentVisual = childVisual;
                    
                    else if (childType == ChildNodeType.PropertyValue && childProp is {} prop)
                        prop.SetValue(contentContainer, childObj, null);

                    break;
                }
                case 0:
                {
                    if (!node.TryGetAttributeValue(nameof(IContentContainer.Content), out var textContent))
                        textContent = node.InnerText;

                    if (!String.IsNullOrEmpty(textContent))
                    {
                        // zb <button>TEXT</button> etc
                        contentVisual = new Label(_visualBootstrapper) {Text = textContent!};
                    }

                    break;
                }
                default:
                    throw new NotImplementedException();
            }
                
            contentContainer.Content = contentVisual;
            return contentContainer;
        }

        private static void AddChildVisual(IVisualElement visual,
                                           IVisualElement childVisual)
        {
            var visualType = visual.GetType();
            
            if (visual is IVisualContainer container)
                container.AddChild(childVisual);
            else
            {
                var addMethod = visualType.GetMethod(nameof(IVisualContainer.AddChild));
                if (addMethod == null)
                    throw new NotImplementedException();

                var mParam = addMethod.GetParameters();
                if (mParam.Length != 1)
                    throw new NotImplementedException();

                var addType = mParam[0].ParameterType;
                var childVisualType = childVisual.GetType();
                if (!addType.IsAssignableFrom(childVisualType))
                    throw new NotImplementedException();

                addMethod.Invoke(visual, new Object[] { childVisual });
            }
        }

        private Object? InflateChild(IMarkupNode childNode,
                                     IVisualElement visual,
                                     Type? dataContextType,
                                     Dictionary<String, String> nameSpaceAssemblySearch,
                                     out ChildNodeType childType,
                                     out PropertyInfo? visualProp)
        {
            var visualType = visual.GetType();

            visualProp = _typeInferrer.FindPublicProperty(visualType, childNode.Name);
            if (visualProp != null)
            {
                childType = ChildNodeType.PropertyValue;

                if (typeof(IVisualElement).IsAssignableFrom(visualProp.PropertyType))
                {
                    // try to set a property with a visual based on the node's value
                    var res = GetVisual(childNode, dataContextType, nameSpaceAssemblySearch);
                    return res;
                }

                if (typeof(IDataTemplate).IsAssignableFrom(visualProp.PropertyType))
                {
                    var dataTemplate = BuildDataTemplate(childNode, dataContextType, nameSpaceAssemblySearch);
                    return dataTemplate;
                }

                if (!typeof(IVisualTemplate).IsAssignableFrom(visualProp.PropertyType) || 
                    childNode.ChildrenCount != 1)
                    throw new NotImplementedException();

                var templateVisual = GetVisual(childNode[0], visualType,
                    nameSpaceAssemblySearch);

                return new VisualTemplate
                {
                    Content = templateVisual
                };
            }

            childType = ChildNodeType.ChildVisual;

            var childVisual = GetVisual(childNode, dataContextType, nameSpaceAssemblySearch);

            return childVisual;
        }

        private IVisualElement InflateXmlImpl(String xml,
                                              IDictionary<String, String> searchSeed)
        {
            var sw = Stopwatch.StartNew();
            
            IMarkupNode? node = XmlNodeBuilder.GetMarkupNode(xml, false);

            if (node == null)
                throw new InvalidOperationException();

            if (node.IsEncodingHeader)
                node = node[0];

            var nsAsmSearch = _visualTypeResolver.GetNamespaceAssemblySearch(node, searchSeed);

            var res = GetVisual(node, null, nsAsmSearch);

            Debug.WriteLine("Inflated visual in " + sw.ElapsedMilliseconds + " ms");
            
            return res;
        }

        private void SetHardCodedValues(IVisualElement visual,
                                        IMarkupNode node,
                                        Dictionary<String, IDataBinding> bindings)
        {
            var vType = visual.GetType();

            foreach (var kvp in node.GetAllAttributes())
            {
                if (bindings.ContainsKey(kvp.Key))
                    continue;

                var prop = vType.GetProperty(kvp.Key);
                if (prop == null)
                    continue;

                var valueConverter = _converterProvider.GetDefaultConverter(prop.PropertyType);

                var propVal = valueConverter != null
                    ? valueConverter.Convert(kvp.Value)
                    : _attributeValueScanner.GetValue(kvp.Value, prop.PropertyType);


                if (propVal != null) prop.SetValue(visual, propVal, null);
            }
        }

        //private static readonly Char[] _namespaceSplitters = {':', '='};
        private readonly IStringPrimitiveScanner _attributeValueScanner;
        private readonly IBindingBuilder _bindingBuilder;
        private readonly IValueConverterProvider _converterProvider;
        private readonly IVisualTypeResolver _visualTypeResolver;

        private readonly ITypeInferrer _typeInferrer;
        private readonly IVisualBootstrapper _visualBootstrapper;

        private static readonly Dictionary<String, String> _defaultNamespaceSeed;
    }
}
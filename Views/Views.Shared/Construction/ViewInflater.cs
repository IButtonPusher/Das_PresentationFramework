using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Construction;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Views
{
    public class ViewInflater : IViewInflater
    {
        public ViewInflater(IVisualBootstrapper visualBootstrapper,
                            IStringPrimitiveScanner xmlAttributeParser,
                            ITypeInferrer typeInferrer,
                            IBindingBuilder bindingBuilder,
                            IValueConverterProvider converterProvider)
        {
            _visualBootstrapper = visualBootstrapper;
            _typeInferrer = typeInferrer;
            _bindingBuilder = bindingBuilder;
            _converterProvider = converterProvider;
            _attributeValueScanner = xmlAttributeParser;
        }

        static ViewInflater()
        {
            _defaultNamespaceSeed = new Dictionary<String, String>();
            _defaultNamespaceSeed["Das.Views.Panels"] = "Das.Views";
            _defaultNamespaceSeed["Das.Views.Controls"] = "Das.Views";
            _defaultNamespaceSeed["Das.Views.DataBinding"] = "Das.Views";
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

            var sw = Stopwatch.StartNew();
            
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
            
            Debug.WriteLine("Read resource in " + sw.ElapsedMilliseconds + " ms");

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


        private static Dictionary<String, String> GetNamespaceAssemblySearch(IMarkupNode node,
                                                                             IDictionary<String, String> search)
        {
            var nsAsmSearch = new Dictionary<String, String>(search);
            return GetNamespaceAssemblySearchImpl(node, nsAsmSearch);
        }

        private static Dictionary<String, String> GetNamespaceAssemblySearchImpl(IMarkupNode node,
            Dictionary<String, String> nsAsmSearch)
        {
            foreach (var attribute in node.GetAllAttributes())
            {
                if (!attribute.Key.StartsWith("xmlns:"))
                    continue;
                var tokens = attribute.Value.Split(';');


                String? asmName = null;
                String? ns = null;

                for (var t = 0; t < tokens.Length; t++)
                {
                    var subTokens = tokens[t].Split(_namespaceSplitters);
                    if (subTokens.Length != 2)
                        continue;

                    switch (subTokens[0])
                    {
                        case "clr-namespace":
                            ns = subTokens[1];
                            break;

                        case "assembly":
                            asmName = subTokens[1];
                            break;
                    }
                }

                if (ns == null)
                    continue;

                if (asmName == null)
                    continue;

                nsAsmSearch[ns] = asmName;
            }

            return nsAsmSearch;
        }


        private Type GetType(IMarkupNode node,
                             String? genericArgName,
                             Dictionary<String, String> nameSpaceAssemblySearch)
        {
            var name = node.Name;

            var notGeneric = _typeInferrer.GetTypeFromClearName(name, nameSpaceAssemblySearch);
            if (notGeneric != null)
                return notGeneric;
            
            if (!String.IsNullOrEmpty(genericArgName) && node.ChildrenCount > 0)
            {
                var letsTry = name + "[" + genericArgName + "]";
                var found = _typeInferrer.GetTypeFromClearName(letsTry, nameSpaceAssemblySearch, true);
                if (found != null)
                    return found;
            }

            //return _typeInferrer.GetTypeFromClearName(name, nameSpaceAssemblySearch)
            //       ?? 
            throw new TypeLoadException(name);
        }

        private Type GetType(IMarkupNode node,
                             String genericArg1,
                             String? genericArg2,
                             Dictionary<String, String> nameSpaceAssemblySearch)
        {
            var name = node.Name;
            
            if (!String.IsNullOrEmpty(genericArg2))
            {
                var genericType = _typeInferrer.GetTypeFromClearName(name + "`2", nameSpaceAssemblySearch)
                                  ?? throw new TypeLoadException(name);
                var genericParam1 = _typeInferrer.GetTypeFromClearName(genericArg1,
                    nameSpaceAssemblySearch, true);
                var genericParam2 = _typeInferrer.GetTypeFromClearName(genericArg2!,
                    nameSpaceAssemblySearch, true);

                try
                {
                    var res = genericType.MakeGenericType(genericParam1, genericParam2);
                    return res;
                }
                catch
                {
                    // ignored
                }

                {
                    var res = genericType.MakeGenericType(genericParam2, genericParam1);
                    return res;
                }
            }

            return GetType(node, genericArg1, nameSpaceAssemblySearch);
        }


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

            Type visualType;


            if (node.TryGetAttributeValue("VisualConstraint", out var visualConstraint))
                visualType = GetType(node, visualConstraint, currentGenericArgName, nameSpaceAssemblySearch);
            else
                visualType = GetType(node, currentGenericArgName, nameSpaceAssemblySearch);

            IVisualElement visual;

            if (typeof(IContentContainer).IsAssignableFrom(visualType))
            {
                if (node.ChildrenCount == 1)
                {
                    var currentNode = node[0];

                    var contentContainer = _visualBootstrapper.Instantiate<IContentContainer>(visualType);

                    var contentVisual = GetVisual(currentNode, dataContextType, nameSpaceAssemblySearch);
                    contentContainer.Content = contentVisual;
                    visual = contentContainer;
                }
                else throw new NotImplementedException();
            }
            else if (visualType is { } validVisualType)
            {
                visual = _visualBootstrapper.Instantiate<IVisualElement>(validVisualType);

                if (node.ChildrenCount > 0) InflateChildNodes(node, visual, dataContextType, nameSpaceAssemblySearch);
            }
            else throw new NotImplementedException();


            if (bindings.Count > 0 && visual is IBindableElement bindable)
                foreach (var binding in bindings)
                    bindable.AddBinding(binding.Value);

            SetHardCodedValues(visual, node, bindings);

            return visual;
        }

        private void InflateChildNodes(IMarkupNode node,
                                       IVisualElement visual,
                                       Type? dataContextType,
                                       Dictionary<String, String> nameSpaceAssemblySearch)
        {
            var visualType = visual.GetType();

            foreach (var childNode in node.Children)
            {
                var visualProp = _typeInferrer.FindPublicProperty(visualType, childNode.Name);
                if (visualProp != null)
                {
                    if (typeof(IVisualElement).IsAssignableFrom(visualProp.PropertyType))
                    {
                        // try to set a property with a visual based on the node's value

                        var childVisual = GetVisual(childNode, dataContextType, nameSpaceAssemblySearch);

                        visualProp.SetValue(visual, childVisual, null);
                    }
                    else if (typeof(IDataTemplate).IsAssignableFrom(visualProp.PropertyType))
                    {
                        var dataTemplate = BuildDataTemplate(childNode, dataContextType, nameSpaceAssemblySearch);
                        visualProp.SetValue(visual, dataTemplate, null);
                    }
                }
                else
                {
                    var childVisual = GetVisual(childNode, dataContextType, nameSpaceAssemblySearch);
                    if (childVisual != null)
                    {
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

                            addMethod.Invoke(visual, new Object[] {childVisual});
                        }
                    }
                }
            }
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

            var nsAsmSearch = GetNamespaceAssemblySearch(node, searchSeed);

            var res = GetVisual(node, null, nsAsmSearch);

            Debug.WriteLine("Inflated visual in " + sw.ElapsedMilliseconds + " ms");
            
            return res;
        }

        private void SetHardCodedValues(IVisualElement visual,
                                        IMarkupNode node,
                                        Dictionary<String, IDataBinding> bindings)
        {
            if (visual is IRepeaterPanel)
            {
            }

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

        private static readonly Char[] _namespaceSplitters = {':', '='};
        private readonly IStringPrimitiveScanner _attributeValueScanner;
        private readonly IBindingBuilder _bindingBuilder;
        private readonly IValueConverterProvider _converterProvider;

        private readonly ITypeInferrer _typeInferrer;
        private readonly IVisualBootstrapper _visualBootstrapper;

        private static readonly Dictionary<String, String> _defaultNamespaceSeed;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Construction;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Views
{
    public class ViewInflater
    {
        public ViewInflater(IVisualBootstrapper visualBootstrapper,
                            ITypeInferrer typeInferrer)
        {
            _visualBootstrapper = visualBootstrapper;
            _typeInferrer = typeInferrer;
        }

        //public IBindableElement<T> Inflate<T>(String xml)
        //{
        //    var tag = XmlNodeBuilder.GetMarkupNode(xml, false);
        //    throw new NotImplementedException();
        //}

        public TVisualElement InflateXml<TVisualElement>(String xml)
        where TVisualElement : IVisualElement
        {
            var visual = InflateXml(xml);
            return (TVisualElement) visual;
        }

        public IVisualElement InflateXml(String xml)
        {
            IMarkupNode? node = XmlNodeBuilder.GetMarkupNode(xml, false);

            if (node == null)
                throw new InvalidOperationException();

            if (node.IsEncodingHeader)
                node = node[0];

            return GetVisual(node, null);
        }

        

        private Type GetType(String name,
                                 String? genericArgName)
        {
            if (!String.IsNullOrEmpty(genericArgName))
            {
                var letsTry = name + "[" + genericArgName + "]";
                return _typeInferrer.GetTypeFromClearName(letsTry, true)
                       ?? throw new TypeLoadException(letsTry);
            }

            return _typeInferrer.GetTypeFromClearName(name)
                   ?? throw new TypeLoadException(name);
        }

        private IVisualElement GetVisual(IMarkupNode node,
                                         Type? dataContextType)
        {
            var bindings = dataContextType != null 
                ? new List<IDataBinding>(GetBindings(dataContextType, node)) 
                : _emptyBindings;

            dataContextType = InferDataContextTypeFromBindings(bindings, dataContextType);

            if (!node.TryGetAttributeValue("ContextType", out var currentGenericArgName))
                currentGenericArgName = dataContextType?.Name;
            else
                dataContextType = _typeInferrer.GetTypeFromClearName(currentGenericArgName, true);

            var visualType = GetType(node.Name, currentGenericArgName);

            IVisualElement visual;

            if (typeof(IContentContainer).IsAssignableFrom(visualType))
            {
                if (node.ChildrenCount == 1)
                {
                    var currentNode = node[0];

                    var contentContainer = _visualBootstrapper.Instantiate<IContentContainer>(visualType);

                    var contentVisual = GetVisual(currentNode, dataContextType);
                    contentContainer.Content = contentVisual;
                    visual = contentContainer;
                }
                else throw new NotImplementedException();
            }
            else if (visualType is { } validVisualType)
            {
                visual = _visualBootstrapper.Instantiate<IVisualElement>(validVisualType);
            }
            else throw new NotImplementedException();

            if (bindings.Count > 0 && visual is IBindableElement bindable)
            {
                foreach (var binding in bindings)
                    bindable.AddBinding(binding);
            }

            return visual;

            //{
            //    currentGenericArg = _typeInferrer.GetTypeFromClearName(contextType, true);
            //}

            
        }

        private static Type? InferDataContextTypeFromBindings(IEnumerable<IDataBinding> bindings,
                                                              Type? currentGenericArg)
        {
            Type? genericChild;

            if (currentGenericArg != null)
            {
                var childArgs = currentGenericArg?.GetGenericArguments();

                if (childArgs != null && childArgs.Length > 0)
                {
                    if (childArgs.Length == 1)
                        genericChild = childArgs[0];
                    else throw new NotImplementedException();
                }

            }

            foreach (var binding in bindings)
            {
                switch (binding)
                {
                    case DeferredPropertyBinding deferredPropertyBinding:

                        switch (deferredPropertyBinding.TargetPropertyName)
                        {
                            case nameof(IItemsControl.ItemsSource) when currentGenericArg != null:
                                var sourceProp = currentGenericArg.GetProperty(
                                    deferredPropertyBinding.SourcePropertyName);

                                if (!(sourceProp?.PropertyType is {} sourcePropType) || 
                                    !typeof(IEnumerable).IsAssignableFrom(sourcePropType) || 
                                    !sourcePropType.IsGenericType)
                                    continue;

                                var srcPropGenerics = sourcePropType.GetGenericArguments();
                                if (srcPropGenerics == null || srcPropGenerics.Length == 0)
                                    continue;

                                if (srcPropGenerics.Length != 1)
                                    throw new NotImplementedException();

                                return srcPropGenerics[0];

                                break;
                        }

                        break;
                }
            }

            return currentGenericArg;
        }

        private static IEnumerable<IDataBinding> GetBindings(Type dataContextType,
                                                             IMarkupNode node)
        {
            foreach (var kvp in node.GetAllAttributes())
            {
                var valTrim = kvp.Value.Trim();
                if (valTrim.Length < 3 ||
                    valTrim[0] != '{' || valTrim[valTrim.Length - 1] != '}')
                    continue;

                var valExpression = valTrim.Substring(1, valTrim.Length - 2);
                var expressionTokens = valExpression.Split();

                if (expressionTokens.Length != 2 || expressionTokens[0] != "Binding")
                    throw new NotImplementedException();

                var propInfo = dataContextType.GetProperty(expressionTokens[1]);
                if (propInfo == null)
                    throw new NotImplementedException();

                var binding = new DeferredPropertyBinding(expressionTokens[1], kvp.Key);
                yield return binding;

                //if (typeof(INotifyPropertyChanged).IsAssignableFrom(dataContextType))
                //{

                //}
            }
        }


       

        private static readonly List<IDataBinding> _emptyBindings = new List<IDataBinding>();
        private readonly ITypeInferrer _typeInferrer;
        private readonly IVisualBootstrapper _visualBootstrapper;
    }
}
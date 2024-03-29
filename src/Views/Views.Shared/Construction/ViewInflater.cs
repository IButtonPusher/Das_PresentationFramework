﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Controls;
using Das.Views.DataBinding;

using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Resources;
using Das.Views.Styles.Construction;
using Das.Views.Templates;

namespace Das.Views.Construction
{
    public partial class ViewInflater : InflaterBase,
                                        IViewInflater
    {

      #if !NET40

       public ViewInflater(IVisualBootstrapper visualBootstrapper,
                           IMultiSerializer serializer,
                           Core.IImageProvider imageProvider)
       {
          _typeManipulator = serializer.TypeManipulator;

          var themeProvider = Views.Styles.BaselineThemeProvider.Instance;
          //var serializer = new DasSerializer();

          _visualBootstrapper = visualBootstrapper;
          _typeInferrer = serializer.TypeInferrer;

          var svgPathBuilder = new Images.Svg.SvgPathBuilder(imageProvider, serializer);

          var resourceBuilder = new ResourceBuilder(imageProvider, svgPathBuilder);
          _bindingBuilder = new BindingBuilder(_typeInferrer, _typeManipulator, 
             serializer.AssemblyList, resourceBuilder);

          
          _converterProvider = new DefaultValueConverterProvider(visualBootstrapper);
          _visualTypeResolver = new VisualTypeResolver(_typeInferrer);

          var declarationWorker = new Styles.DeclarationWorker();

          var variableAccessor = new Styles.StyleVariableAccessor(themeProvider.ColorPalette);
          var inflator = new DefaultStyleInflater(serializer.TypeInferrer, variableAccessor);
          var styleProvider = new VisualStyleProvider(inflator);

          _appliedStyleBuilder = new Views.Styles.AppliedRuleBuilder(styleProvider, declarationWorker,
             _typeManipulator);

          _attributeValueScanner = serializer.AttributeParser;
       }

#endif

        public ViewInflater(IVisualBootstrapper visualBootstrapper,
                            IStringPrimitiveScanner xmlAttributeParser,
                            ITypeInferrer typeInferrer,
                            IBindingBuilder bindingBuilder,
                            IValueConverterProvider converterProvider,
                            IVisualTypeResolver visualTypeResolver,
                            IPropertyProvider typeManipulator,
                            IAppliedStyleBuilder appliedStyleBuilder)
        {
            _visualBootstrapper = visualBootstrapper;
            _typeInferrer = typeInferrer;
            _bindingBuilder = bindingBuilder;
            _converterProvider = converterProvider;
            _visualTypeResolver = visualTypeResolver;
            _typeManipulator = typeManipulator;
            _appliedStyleBuilder = appliedStyleBuilder;
            _attributeValueScanner = xmlAttributeParser;
        }

        public Task<IVisualElement> GetVisualAsync(IMarkupNode node,
                                                   Type dataContextType,
                                                   IVisualLineage visualLineage,
                                                   ApplyVisualStyles applyStyles)
        {
            return GetVisualAsync(node, dataContextType, VisualTypeResolver.DefaultNamespaceSeed,
                visualLineage, applyStyles);
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

                addMethod.Invoke(visual, new Object[] {childVisual});
            }
        }

        private async Task<IContentVisual> BuildContentVisualAsync(IMarkupNode node,
                                                                   Type? dataContextType,
                                                                   Dictionary<String, String> nameSpaceAssemblySearch,
                                                                   Type visualType,
                                                                   IVisualLineage visualLineage,
                                                                   ApplyVisualStyles applyStyles)
        {
            IVisualElement? contentVisual = null;

            //-------------------------------
            var contentContainer = _visualBootstrapper.Instantiate<IContentVisual>(visualType)
                                   ?? throw new InvalidOperationException();
            //await applyStyles(contentContainer, node, visualLineage, this);
            //-------------------------------

            visualLineage.PushVisual(contentContainer);

            switch (node.ChildrenCount)
            {
                case 1:
                {
                    var currentNode = node[0];
                    var childObjRes = await InflateChildAsync(currentNode, contentContainer,
                            dataContextType, nameSpaceAssemblySearch, visualLineage, applyStyles)
                        .ConfigureAwait(false);

                    if (childObjRes.ChildType == ChildNodeType.ChildVisual &&
                        childObjRes.Child is IVisualElement childVisual)
                        contentVisual = childVisual;

                    else if (childObjRes.ChildType == ChildNodeType.PropertyValue &&
                             childObjRes.VisualProperty is { } prop)
                    {
                        Object oContentContainer = contentContainer;
                        prop.SetPropertyValue(ref oContentContainer, childObjRes.Child);
                        //prop.SetValue(contentContainer, childObjRes.Child, null);
                    }

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
                        visualLineage.PushVisual(contentVisual);
                    }

                    break;
                }
                default:
                    throw new NotImplementedException();
            }


            contentContainer.Content = contentVisual;

            if (contentVisual != null)
                visualLineage.AssertPopVisual(contentVisual);


            return contentContainer;
        }

        private async Task<IDataTemplate> BuildDataTemplateAsync(IMarkupNode node,
                                                                 Type? dataContextType,
                                                                 Dictionary<String, String> nameSpaceAssemblySearch,
                                                                 IVisualLineage visualLineage,
                                                                 ApplyVisualStyles applyStyles)
        {
            if (node.ChildrenCount != 1)
            {
                var visuals = new List<IVisualElement>();

                foreach (var childNode in node.Children)
                {
                    var visual = await GetVisualAsync(childNode, dataContextType,
                        nameSpaceAssemblySearch, visualLineage, applyStyles).ConfigureAwait(false);
                    visuals.Add(visual);
                }

                return new MultiDataTemplate(_visualBootstrapper, dataContextType, visuals);
            }

            var onlyChild = node.Children.First();

            switch (onlyChild.Name)
            {
                case nameof(MultiDataTemplate):
                    var dataBindings = (await _bindingBuilder.GetBindingsDictionaryAsync(node,
                        dataContextType, nameSpaceAssemblySearch)).Values.OfType<IDataBinding>();

                    dataContextType = _bindingBuilder.InferDataContextTypeFromBindings(dataBindings,
                        dataContextType);
                    return await BuildDataTemplateAsync(onlyChild, dataContextType,
                        nameSpaceAssemblySearch, visualLineage, applyStyles).ConfigureAwait(false);

                case nameof(DataTemplate):
                    return await BuildDataTemplateAsync(onlyChild, dataContextType,
                        nameSpaceAssemblySearch, visualLineage, applyStyles).ConfigureAwait(false);

                default:
                    var visualContent = await GetVisualAsync(onlyChild, dataContextType,
                        nameSpaceAssemblySearch, visualLineage, applyStyles).ConfigureAwait(false);
                    visualLineage.AssertPopVisual(visualContent);
                    return new DataTemplate(_visualBootstrapper, dataContextType, visualContent);
            }
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

        private Object? GetPropertyValue(PropertyInfo prop,
                                         String strValue)
        {
            var useType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            if (useType == typeof(QuantifiedDouble))
                return QuantifiedDouble.Parse(strValue);

            var valueConverter = _converterProvider.GetDefaultConverter(useType);

            var propVal = valueConverter != null
                ? valueConverter.Convert(strValue)
                : _attributeValueScanner.GetValue(strValue, useType, false);

            return propVal;
        }

        /// <summary>
        ///     Builds a visual from a markup node.  Infers the data context type and instantiates
        ///     a generic visual if possible
        /// </summary>
        /// <param name="node">an xml/json etc node</param>
        /// <param name="dataContextType">the data context type of the parent visual.</param>
        /// <param name="nameSpaceAssemblySearch"></param>
        /// <param name="visualLineage"></param>
        /// <param name="applyStyles"></param>
        /// <returns></returns>
        private async Task<IVisualElement> GetVisualAsync(IMarkupNode node,
                                                          Type? dataContextType,
                                                          Dictionary<String, String> nameSpaceAssemblySearch,
                                                          IVisualLineage visualLineage,
                                                          ApplyVisualStyles applyStyles)
        {
            var bindings = await _bindingBuilder.GetBindingsDictionaryAsync(node,
                dataContextType, nameSpaceAssemblySearch);

            var dataBindings = bindings.Values.OfType<IDataBinding>()
                                       .ToArray();

            dataContextType = _bindingBuilder.InferDataContextTypeFromBindings(dataBindings,
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
                // CONTENT VISUAL
                visual = await BuildContentVisualAsync(node, dataContextType,
                        nameSpaceAssemblySearch, visualType, visualLineage, applyStyles)
                    .ConfigureAwait(false);
            else if (visualType is { } validVisualType)
            {
                //-------------------------------
                visual = _visualBootstrapper.Instantiate<IVisualElement>(validVisualType);
                //await applyStyles(visual, node, visualLineage, this);
                //-------------------------------

                visualLineage.PushVisual(visual);

                if (node.ChildrenCount > 0)
                    // PANEL
                    await InflateAndAddChildNodesAsync(node, visual, dataContextType,
                        nameSpaceAssemblySearch, visualLineage,
                        _appliedStyleBuilder.ApplyVisualStylesAsync).ConfigureAwait(false);

                if (node.InnerText is { } innerText &&
                    innerText.Trim() is { } validInnerText && validInnerText.Length > 0 &&
                    GetAttribute<ContentPropertyAttribute>(validVisualType) is { } cp &&
                    _typeInferrer.FindPublicProperty(validVisualType, cp.Name) is { } contentProp &&
                    contentProp.PropertyType == typeof(String))
                    // zb <Label>hello world</Label>
                    contentProp.SetValue(visual, validInnerText, null);
            }
            else throw new NotImplementedException();

            if (dataBindings.Length > 0 && visual is IBindableElement bindable)
                foreach (var binding in dataBindings)
                {
                    bindable.AddBinding(binding);
                }

            //////////////////////////////////////////
            // have to apply styles after children are instantiated as some styles specify child selectors
            await applyStyles(visual, node, visualLineage, this, _visualBootstrapper);
            //////////////////////////////////////////

            SetHardCodedValues(visual, node, bindings);

            return visual;
        }

        private async Task InflateAndAddChildNodesAsync(IMarkupNode node,
                                                        IVisualElement visual,
                                                        Type? dataContextType,
                                                        Dictionary<String, String> nameSpaceAssemblySearch,
                                                        IVisualLineage visualLineage,
                                                        ApplyVisualStyles applyStyles)
        {
            foreach (var childNode in node.Children)
            {
                var childObjRes = await InflateChildAsync(childNode, visual,
                        dataContextType, nameSpaceAssemblySearch, visualLineage, applyStyles)
                    .ConfigureAwait(false);

                if (childObjRes.ChildType == ChildNodeType.PropertyValue &&
                    childObjRes.VisualProperty is { } prop)
                {
                    // visual is a property value, not a child visual
                    Object oVisual = visual;
                    prop.SetPropertyValue(ref oVisual, childObjRes.Child);
                    //prop.SetValue(visual, childObjRes.Child, null);
                }
                else if (childObjRes.Child is IVisualElement childVisual)
                    // panel's child visual
                    AddChildVisual(visual, childVisual);

                if (childObjRes.Child is IVisualElement childVisualElement)
                    visualLineage.AssertPopVisual(childVisualElement);
            }
        }

        private async Task<ChildObjectResult> InflateChildAsync(IMarkupNode childNode,
                                                                IVisualElement visual,
                                                                Type? dataContextType,
                                                                Dictionary<String, String> nameSpaceAssemblySearch,
                                                                IVisualLineage visualLineage,
                                                                ApplyVisualStyles applyStyles)
        {
            var visualType = visual.GetType();
            ChildNodeType childType;

            var visualProp = _typeInferrer.FindPublicProperty(visualType, childNode.Name);
            IPropertyAccessor? propAccessor = null;

            if (visualProp != null)
            {
                propAccessor = _typeManipulator.GetPropertyAccessor(visualType, childNode.Name);

                childType = ChildNodeType.PropertyValue;

                if (typeof(IVisualElement).IsAssignableFrom(visualProp.PropertyType))
                {
                    // try to set a property with a visual based on the node's value
                    var res = await GetVisualAsync(childNode, dataContextType,
                        nameSpaceAssemblySearch, visualLineage, applyStyles).ConfigureAwait(false);
                    {
                        return new ChildObjectResult(res, childType, propAccessor); // visualProp);
                    }
                }

                if (typeof(IDataTemplate).IsAssignableFrom(visualProp.PropertyType))
                {
                    var dataTemplate = await BuildDataTemplateAsync(childNode, dataContextType,
                        nameSpaceAssemblySearch, visualLineage, applyStyles).ConfigureAwait(false);
                    return new ChildObjectResult(dataTemplate, childType, propAccessor); // visualProp);
                }

                if (!typeof(IVisualTemplate).IsAssignableFrom(visualProp.PropertyType) ||
                    childNode.ChildrenCount != 1)
                    throw new NotImplementedException();

                var templateVisual = await GetVisualAsync(childNode[0], visualType,
                    nameSpaceAssemblySearch, visualLineage, applyStyles).ConfigureAwait(false);

                var vt = new VisualTemplate
                {
                    Content = templateVisual
                };

                visualLineage.AssertPopVisual(templateVisual);

                return new ChildObjectResult(vt, childType, propAccessor); // visualProp);
            }

            childType = ChildNodeType.ChildVisual;

            var childVisual = await GetVisualAsync(childNode, dataContextType,
                nameSpaceAssemblySearch, visualLineage, applyStyles).ConfigureAwait(false);

            return new ChildObjectResult(childVisual, childType, propAccessor); // visualProp);
        }

        private void SetHardCodedValues(IVisualElement visual,
                                        IMarkupNode node,
                                        Dictionary<String, IPropertyBinding> bindings)
        {
            //var vType = visual.GetType();

            foreach (var kvp in node.GetAllAttributes())
            {
                SetHardCodedValue(visual, kvp, bindings);

                //if (bindings.TryGetValue(kvp.Key, out var binding))
                //{
                //    if (binding is EmbeddedResourceBinding resourceBinding)

                //    continue;
                //}

                //var prop = _typeInferrer.FindPublicProperty(vType, kvp.Key);

                //if (prop == null)
                //    continue;

                //var propVal = GetPropertyValue(prop, kvp.Value);

                ////var valueConverter = _converterProvider.GetDefaultConverter(prop.PropertyType);

                ////var propVal = valueConverter != null
                ////    ? valueConverter.Convert(kvp.Value)
                ////    : _attributeValueScanner.GetValue(kvp.Value, prop.PropertyType);


                //if (propVal != null)
                //    prop.SetValue(visual, propVal, null);
            }
        }

        private void SetHardCodedValue(IVisualElement visual,
                                       KeyValuePair<String, String> kvp,
                                       Dictionary<String, IPropertyBinding> bindings)
        {
            var prop = _typeInferrer.FindPublicProperty(visual.GetType(), kvp.Key);

            if (prop == null)
                return;

            Object? propVal;

            if (bindings.TryGetValue(kvp.Key, out var binding))
            {
                if (binding is EmbeddedResourceBinding resourceBinding)
                {
                    propVal = resourceBinding.Value;
                }
                else
                    return;
            }
            else
                propVal = GetPropertyValue(prop, kvp.Value);

            //var valueConverter = _converterProvider.GetDefaultConverter(prop.PropertyType);

            //var propVal = valueConverter != null
            //    ? valueConverter.Convert(kvp.Value)
            //    : _attributeValueScanner.GetValue(kvp.Value, prop.PropertyType);


            if (propVal != null)
                prop.SetValue(visual, propVal, null);
        }

        //private static readonly Dictionary<String, String> _defaultNamespaceSeed;
        private readonly IAppliedStyleBuilder _appliedStyleBuilder;


        private readonly IStringPrimitiveScanner _attributeValueScanner;
        private readonly IBindingBuilder _bindingBuilder;
        private readonly IValueConverterProvider _converterProvider;
        //private readonly IStyledVisualBuilder _styledVisualBuilder;
        private readonly ITypeInferrer _typeInferrer;
        private readonly IPropertyProvider _typeManipulator;
        private readonly IVisualBootstrapper _visualBootstrapper;
        private readonly IVisualTypeResolver _visualTypeResolver;
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Container;
using Das.Serializer;
using Das.Views.Colors;
using Das.Views.Construction;
using Das.Views.Construction.Styles;
using Das.Views.Controls;
using Das.Views.Core;
using Das.Views.Core.Geometry;
using Das.Views.Images;
using Das.Views.Layout;
using Das.Views.Styles;
using Das.Views.Styles.Construction;

using Das.Views.Templates;

namespace Das.Views
{
    public abstract class BaseRenderKit : IVisualSurrogateProvider
    {
        //protected BaseRenderKit(IThemeProvider themeProvider,
        //                        IStringPrimitiveScanner attributeScanner,
        //                        ITypeInferrer typeInferrer,
        //                        IPropertyProvider propertyProvider,
        //                        Dictionary<IVisualElement, ValueCube> renderPositions,
        //                        IImageProvider imageProvider,
        //                        IAssemblyList assemblyList,
        //                        IXmlSerializer xmlSerializer)
        //    : this(new BaseResolver(), themeProvider, attributeScanner,
        //        typeInferrer, propertyProvider, renderPositions,
        //        GetAppliedStyleBuilder(propertyProvider, GetStyleProvider(typeInferrer, themeProvider)), 
        //        imageProvider, assemblyList, xmlSerializer)
        //{
        //}

        //protected BaseRenderKit(IResolver resolver,
        //                        IThemeProvider themeProvider,
        //                        IStringPrimitiveScanner attributeScanner,
        //                        ITypeInferrer typeInferrer,
        //                        IPropertyProvider propertyProvider,
        //                        Dictionary<IVisualElement, ValueCube> renderPositions,
        //                        IImageProvider imageProvider,
        //                        IAssemblyList assemblyList,
        //                        IXmlSerializer xmlSerializer)
        //: this(resolver, themeProvider, attributeScanner, typeInferrer, propertyProvider, renderPositions,
        //    GetAppliedStyleBuilder(propertyProvider, GetStyleProvider(typeInferrer, themeProvider)), 
        //    imageProvider, assemblyList, xmlSerializer)
        //{}

        //protected BaseRenderKit(IResolver resolver,
        //                        IThemeProvider themeProvider,
        //                        IStringPrimitiveScanner attributeScanner,
        //                        ITypeInferrer typeInferrer,
        //                        IPropertyProvider propertyProvider,
        //                        Dictionary<IVisualElement, ValueCube> renderPositions,
        //                        IAppliedStyleBuilder styleBuilder,
        //                        IImageProvider imageProvider,
        //                        IAssemblyList assemblyList,
        //                        IXmlSerializer xmlSerializer)
        //: this(resolver, attributeScanner, typeInferrer, propertyProvider, 
        //    GetVisualBootstrapper(resolver, themeProvider, propertyProvider, styleBuilder), 
        //    renderPositions, styleBuilder.StyleProvider, imageProvider, assemblyList, xmlSerializer)
        //{

        //}

        //protected BaseRenderKit(IResolver resolver,
        //                        IStringPrimitiveScanner attributeScanner,
        //                        ITypeInferrer typeInferrer,
        //                        IPropertyProvider propertyProvider,
        //                        IVisualBootstrapper visualBootstrapper,
        //                        Dictionary<IVisualElement, ValueCube> renderPositions,
        //                        IVisualStyleProvider styleProvider,
        //                        IImageProvider imageProvider,
        //                        IAssemblyList assemblyList,
        //                        IXmlSerializer xmlSerializer)
        //: this(resolver, 
        //    attributeScanner, 
        //    typeInferrer, 
        //    propertyProvider,
        //    visualBootstrapper,
        //    GetAppliedStyleBuilder(propertyProvider, styleProvider),
        //    renderPositions, imageProvider, assemblyList, xmlSerializer)

        //{

        //}

        //protected BaseRenderKit(IResolver resolver,
        //                        IStringPrimitiveScanner attributeScanner,
        //                        ITypeInferrer typeInferrer,
        //                        IPropertyProvider propertyProvider,
        //                        IVisualBootstrapper visualBootstrapper,
        //                        IAppliedStyleBuilder appliedStyleBuilder,
        //                        Dictionary<IVisualElement, ValueCube> renderPositions,
        //                        IImageProvider imageProvider,
        //                        IAssemblyList assemblyList,
        //                        IXmlSerializer xmlSerializer)
        //    : this(resolver, visualBootstrapper, 
        //        GetViewInflater(visualBootstrapper, attributeScanner, 
        //            typeInferrer, propertyProvider, appliedStyleBuilder, 
        //            assemblyList, imageProvider, xmlSerializer),
        //        renderPositions, imageProvider)

        //{

        //}

        protected BaseRenderKit(IResolver resolver,
                                IVisualBootstrapper visualBootstrapper,
                                IViewInflater viewInflater,
                                Dictionary<IVisualElement, ValueCube> renderPositions,
                                IImageProvider imageProvider)
        {
            _renderPositions = renderPositions;
            ImageProvider = imageProvider;
            Container = resolver;

            _surrogateInstances = new Dictionary<IVisualElement, IVisualSurrogate>();
            _surrogateTypeBuilders = new Dictionary<Type, Func<IVisualElement, IVisualSurrogate>>();

            VisualBootstrapper = visualBootstrapper;

            ViewInflater = viewInflater;

            resolver.ResolveTo(visualBootstrapper);
        }


        protected BaseRenderKit(IImageProvider imageProvider,
                                IMultiSerializer xmlSerializer,
                                ISvgPathBuilder svgPathBuilder)
            : this(imageProvider, xmlSerializer, new BaseResolver(),
                BaselineThemeProvider.Instance,
                GetStyleProvider(xmlSerializer.TypeInferrer, BaselineThemeProvider.Instance),
                svgPathBuilder)
        {

        }

        private BaseRenderKit(IImageProvider imageProvider,
                              IMultiSerializer xmlSerializer,
                              IResolver resolver,
                              IThemeProvider themeProvider,
                              IVisualStyleProvider styleProvider,
                              ISvgPathBuilder svgPathBuilder)
            : this(imageProvider, xmlSerializer, resolver, themeProvider,
                GetAppliedStyleBuilder(xmlSerializer.TypeManipulator, styleProvider), svgPathBuilder)
        {

        }

        private BaseRenderKit(IImageProvider imageProvider,
                              IMultiSerializer xmlSerializer,
                              IResolver resolver,
                              IThemeProvider themeProvider,
                              IAppliedStyleBuilder appliedStyleBuilder,
                              ISvgPathBuilder svgPathBuilder)
        : this(imageProvider, xmlSerializer, resolver, appliedStyleBuilder,
            GetVisualBootstrapper(resolver, themeProvider, xmlSerializer.TypeManipulator,
                appliedStyleBuilder), svgPathBuilder)
        {

        }

        private BaseRenderKit(IImageProvider imageProvider,
                              IMultiSerializer xmlSerializer,
                              IResolver resolver,
                              IAppliedStyleBuilder appliedStyleBuilder,
                              IVisualBootstrapper visualBootstrapper,
                              ISvgPathBuilder svgPathBuilder)
        : this(resolver, visualBootstrapper, 
            GetViewInflater(visualBootstrapper, xmlSerializer.AttributeParser,
                xmlSerializer.TypeInferrer, xmlSerializer.TypeManipulator,
                appliedStyleBuilder, xmlSerializer.AssemblyList, imageProvider,
                xmlSerializer, svgPathBuilder), 
            new Dictionary<IVisualElement, ValueCube>(), imageProvider)
        {

        }

     
        private static IVisualBootstrapper GetVisualBootstrapper(IResolver resolver,
                                                                 IThemeProvider themeProvider,
                                                                 IPropertyProvider propertyProvider,
                                                                 IAppliedStyleBuilder styleBuilder)
        {
            return new DefaultVisualBootstrapper(resolver, themeProvider, propertyProvider,
                new LayoutQueue(), styleBuilder);
        }

        private static IAppliedStyleBuilder GetAppliedStyleBuilder(IPropertyProvider propertyProvider,
                                                                   IVisualStyleProvider styleProvider)
        {
            var declarationWorker = new DeclarationWorker();
            var appliedStyleBuilder = new AppliedRuleBuilder(styleProvider, declarationWorker,
                propertyProvider);

            return appliedStyleBuilder;
        }

        private static IVisualStyleProvider GetStyleProvider(ITypeInferrer typeInferrer,
                                                             IThemeProvider themeProvider)
        {
            var variableAccessor = new StyleVariableAccessor(themeProvider.ColorPalette);

            var styleInflater = new DefaultStyleInflater(typeInferrer, variableAccessor);
            var styleProvider = new VisualStyleProvider(styleInflater, themeProvider,
                new BasePrimitiveStyle(variableAccessor));
            return styleProvider;
        }

        private static IViewInflater GetViewInflater(IVisualBootstrapper visualBootstrapper,
                                                     IStringPrimitiveScanner attributeScanner,
                                                     ITypeInferrer typeInferrer,
                                                     IPropertyProvider propertyProvider,
                                                     IAppliedStyleBuilder appliedStyleBuilder,
                                                     IAssemblyList assemblies,
                                                     IImageProvider imageProvider,
                                                     IXmlSerializer serializer,
                                                     ISvgPathBuilder svgPathBuilder)
        {
            var resourceBuilder = new ResourceBuilder(imageProvider, serializer, svgPathBuilder);

            var bindingBuilder = new BindingBuilder(typeInferrer, propertyProvider, 
                assemblies, resourceBuilder);
            var converterProvider = new DefaultValueConverterProvider(visualBootstrapper);
            var visualTypeResolver = new VisualTypeResolver(typeInferrer);

            return new ViewInflater(visualBootstrapper, attributeScanner, 
                typeInferrer, bindingBuilder, converterProvider, 
                visualTypeResolver, 
                propertyProvider, appliedStyleBuilder);
        }

        public Boolean TrySetSurrogate(ref IVisualElement element)
        {
            if (_surrogateInstances.TryGetValue(element, out var surrogate))
            {
                element = surrogate;
                return true;
            }

            if (_surrogateTypeBuilders.TryGetValue(element.GetType(), out var bldr))
            {
                var res = bldr(element);
                _surrogateInstances[element] = res;
                element = res;
                return true;
            }

            return false;
        }

        public IResolver Container { get; }


        public IViewInflater ViewInflater { get; }


        public virtual IVisualBootstrapper VisualBootstrapper { get; }

        public IImageProvider ImageProvider { get; }

        public virtual void RegisterSurrogate<T>(Func<IVisualElement, IVisualSurrogate> builder)
            where T : IVisualElement
        {
            _surrogateTypeBuilders[typeof(T)] = builder;
        }
        
        protected readonly Dictionary<IVisualElement, ValueCube> _renderPositions;
        private readonly Dictionary<IVisualElement, IVisualSurrogate> _surrogateInstances;
        private readonly Dictionary<Type, Func<IVisualElement, IVisualSurrogate>> _surrogateTypeBuilders;
    }
}
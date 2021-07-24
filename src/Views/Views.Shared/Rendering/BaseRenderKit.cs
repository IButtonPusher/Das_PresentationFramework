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
        

        protected BaseRenderKit(IResolver resolver,
                                IVisualBootstrapper visualBootstrapper,
                                IViewInflater viewInflater,
                                Dictionary<IVisualElement, ValueCube> renderPositions,
                                IImageProvider imageProvider)
        : this(ref resolver!, imageProvider, renderPositions, new LayoutQueue())
        {
            VisualBootstrapper = visualBootstrapper;

            ViewInflater = viewInflater;

            Container.ResolveTo(visualBootstrapper);
        }


        protected BaseRenderKit(IImageProvider imageProvider,
                                IMultiSerializer xmlSerializer,
                                ISvgPathBuilder svgPathBuilder,
                                IResolver? resolver,
                                IThemeProvider themeProvider)
        : this(imageProvider, xmlSerializer, svgPathBuilder, resolver,
           new LayoutQueue(), themeProvider)
        {

        }

        protected BaseRenderKit(IImageProvider imageProvider,
                                IMultiSerializer xmlSerializer,
                                ISvgPathBuilder svgPathBuilder,
                                IResolver? resolver,
                                ILayoutQueue layoutQueue,
                                IThemeProvider themeProvider)
            : this(ref resolver,
                imageProvider, new Dictionary<IVisualElement, ValueCube>(),
                layoutQueue)
        {

            var styleProvider = GetStyleProvider(xmlSerializer.TypeInferrer, themeProvider);

            var appliedStyleBuilder = GetAppliedStyleBuilder(xmlSerializer.TypeManipulator, styleProvider);
            var visualBootstrapper = GetVisualBootstrapper(Container, themeProvider,
               //BaselineThemeProvider.Instance,
                xmlSerializer.TypeManipulator, appliedStyleBuilder, layoutQueue);

            VisualBootstrapper = visualBootstrapper;

            ViewInflater = GetViewInflater(visualBootstrapper, xmlSerializer.AttributeParser,
                xmlSerializer.TypeInferrer, xmlSerializer.TypeManipulator,
                appliedStyleBuilder, xmlSerializer.AssemblyList, imageProvider, svgPathBuilder);
            
            Container.ResolveTo(visualBootstrapper);
            Container.ResolveTo(svgPathBuilder);
        }

#pragma warning disable 8618

        private BaseRenderKit(ref IResolver? resolver,
                              IImageProvider imageProvider, 
                              Dictionary<IVisualElement, ValueCube> renderPositions,
                              ILayoutQueue layoutQueue)
        {
            resolver ??= new BaseResolver();

            Container = resolver;

            ImageProvider = imageProvider;

            _renderPositions = renderPositions;
            _layoutQueue = layoutQueue;
            _surrogateInstances = new Dictionary<IVisualElement, IVisualSurrogate>();
            _surrogateTypeBuilders = new Dictionary<Type, Func<IVisualElement, IVisualSurrogate>>();
        }

#pragma warning restore 8618

       
        private static IVisualBootstrapper GetVisualBootstrapper(IResolver resolver,
                                                                 IThemeProvider themeProvider,
                                                                 IPropertyProvider propertyProvider,
                                                                 IAppliedStyleBuilder styleBuilder,
                                                                 ILayoutQueue layoutQueue)
        {
            return new DefaultVisualBootstrapper(resolver, themeProvider, propertyProvider,
               layoutQueue, styleBuilder);
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
            var styleProvider = new VisualStyleProvider(styleInflater,
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
                                                     ISvgPathBuilder svgPathBuilder)
        {
            var resourceBuilder = new ResourceBuilder(imageProvider, svgPathBuilder);

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
        protected readonly ILayoutQueue _layoutQueue;
        private readonly Dictionary<IVisualElement, IVisualSurrogate> _surrogateInstances;
        private readonly Dictionary<Type, Func<IVisualElement, IVisualSurrogate>> _surrogateTypeBuilders;
    }
}
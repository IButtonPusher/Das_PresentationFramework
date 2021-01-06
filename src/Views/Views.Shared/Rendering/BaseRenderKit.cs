using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Container;
using Das.Serializer;
using Das.Views.Colors;
using Das.Views.Construction;
using Das.Views.Construction.Styles;
using Das.Views.Controls;
using Das.Views.Core.Geometry;
using Das.Views.Layout;
using Das.Views.Styles;
using Das.Views.Styles.Construction;
using Das.Views.Templates;

namespace Das.Views
{
    public abstract class BaseRenderKit : IVisualSurrogateProvider
    {
        protected BaseRenderKit(IThemeProvider themeProvider,
                                IStringPrimitiveScanner attributeScanner,
                                ITypeInferrer typeInferrer,
                                IPropertyProvider propertyProvider,
                                Dictionary<IVisualElement, ValueCube> renderPositions)
            : this(new BaseResolver(), themeProvider, attributeScanner,
                typeInferrer, propertyProvider, renderPositions)
        {
        }

        protected BaseRenderKit(IResolver resolver,
                                IThemeProvider themeProvider,
                                IStringPrimitiveScanner attributeScanner,
                                ITypeInferrer typeInferrer,
                                IPropertyProvider propertyProvider,
                                Dictionary<IVisualElement, ValueCube> renderPositions)
        : this(resolver, themeProvider, attributeScanner, typeInferrer, propertyProvider, 
            GetVisualBootstrapper(resolver, themeProvider, propertyProvider), renderPositions)
        {
           
        }

        protected BaseRenderKit(IResolver resolver,
                                IThemeProvider themeProvider,
                                IStringPrimitiveScanner attributeScanner,
                                ITypeInferrer typeInferrer,
                                IPropertyProvider propertyProvider,
                                IVisualBootstrapper visualBootstrapper,
                                Dictionary<IVisualElement, ValueCube> renderPositions)
        : this(resolver, 
            attributeScanner, 
            typeInferrer, 
            propertyProvider,
            visualBootstrapper,
            GetAppliedStyleBuilder(visualBootstrapper, typeInferrer, propertyProvider, themeProvider),
            //GetStyleVisualBuilder(visualBootstrapper, typeInferrer, propertyProvider),
            renderPositions)
        
        {

        }

        protected BaseRenderKit(IResolver resolver,
                                IStringPrimitiveScanner attributeScanner,
                                ITypeInferrer typeInferrer,
                                IPropertyProvider propertyProvider,
                                IVisualBootstrapper visualBootstrapper,
                                IAppliedStyleBuilder appliedStyleBuilder,
                                Dictionary<IVisualElement, ValueCube> renderPositions)
            : this(resolver, visualBootstrapper, 
                GetViewInflater(visualBootstrapper, attributeScanner, 
                    typeInferrer, propertyProvider, appliedStyleBuilder),// styledVisualBuilder),
                renderPositions)
        
        {

        }
        
        protected BaseRenderKit(IResolver resolver,
                                IVisualBootstrapper visualBootstrapper,
                                IViewInflater viewInflater,
                                Dictionary<IVisualElement, ValueCube> renderPositions)
        {
            //_styleContext = styleContext;
            _renderPositions = renderPositions;
            Container = resolver;
            
            _surrogateInstances = new Dictionary<IVisualElement, IVisualSurrogate>();
            _surrogateTypeBuilders = new Dictionary<Type, Func<IVisualElement, IVisualSurrogate>>();
            
            VisualBootstrapper = visualBootstrapper;

            ViewInflater = viewInflater;

            resolver.ResolveTo(visualBootstrapper);
        }
        
        private static IVisualBootstrapper GetVisualBootstrapper(IResolver resolver,
                                                                 IThemeProvider themeProvider,
                                                                 IPropertyProvider propertyProvider)
        {
            return new DefaultVisualBootstrapper(resolver, themeProvider, propertyProvider,
                new LayoutQueue());
        }

        private static IAppliedStyleBuilder GetAppliedStyleBuilder(IVisualBootstrapper visualBootstrapper,
                                                                   ITypeInferrer typeInferrer,
                                                                   IPropertyProvider propertyProvider,
                                                                   IThemeProvider themeProvider)
        {
            var styleInflater = new DefaultStyleInflater(typeInferrer);
            var styleProvider = new VisualStyleProvider(styleInflater, themeProvider);
            var declarationWorker = new DeclarationWorker(visualBootstrapper);
            var appliedStyleBuilder = new AppliedRuleBuilder(styleProvider, declarationWorker,
                propertyProvider);

            return appliedStyleBuilder;
        }

        private static IViewInflater GetViewInflater(IVisualBootstrapper visualBootstrapper,
                                                     IStringPrimitiveScanner attributeScanner,
                                                     ITypeInferrer typeInferrer,
                                                     IPropertyProvider propertyProvider,
                                                     
                                                     IAppliedStyleBuilder appliedStyleBuilder)
        {
            var bindingBuilder = new BindingBuilder(typeInferrer, propertyProvider);
            var converterProvider = new DefaultValueConverterProvider(visualBootstrapper);
            var visualTypeResolver = new VisualTypeResolver(typeInferrer);

            return new ViewInflater(visualBootstrapper, attributeScanner, 
                typeInferrer, bindingBuilder, converterProvider, 
                visualTypeResolver, //styleVisualBuilder, 
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
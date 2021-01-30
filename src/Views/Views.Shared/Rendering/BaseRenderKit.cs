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
using Das.Views.Styles.DefaultStyles;
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
                typeInferrer, propertyProvider, renderPositions,
                GetAppliedStyleBuilder(propertyProvider, GetStyleProvider(typeInferrer, themeProvider)))
        {
        }

        protected BaseRenderKit(IResolver resolver,
                                IThemeProvider themeProvider,
                                IStringPrimitiveScanner attributeScanner,
                                ITypeInferrer typeInferrer,
                                IPropertyProvider propertyProvider,
                                Dictionary<IVisualElement, ValueCube> renderPositions)
        : this(resolver, themeProvider, attributeScanner, typeInferrer, propertyProvider, renderPositions,
            GetAppliedStyleBuilder(propertyProvider, GetStyleProvider(typeInferrer, themeProvider)))
            //GetStyleProvider(typeInferrer, themeProvider))
        {}

        protected BaseRenderKit(IResolver resolver,
                                IThemeProvider themeProvider,
                                IStringPrimitiveScanner attributeScanner,
                                ITypeInferrer typeInferrer,
                                IPropertyProvider propertyProvider,
                                Dictionary<IVisualElement, ValueCube> renderPositions,
                                IAppliedStyleBuilder styleBuilder)
        : this(resolver, attributeScanner, typeInferrer, propertyProvider, 
            GetVisualBootstrapper(resolver, themeProvider, propertyProvider, styleBuilder), 
            renderPositions, styleBuilder.StyleProvider)
        {
           
        }

        protected BaseRenderKit(IResolver resolver,
                                IStringPrimitiveScanner attributeScanner,
                                ITypeInferrer typeInferrer,
                                IPropertyProvider propertyProvider,
                                IVisualBootstrapper visualBootstrapper,
                                Dictionary<IVisualElement, ValueCube> renderPositions,
                                IVisualStyleProvider styleProvider)
        : this(resolver, 
            attributeScanner, 
            typeInferrer, 
            propertyProvider,
            visualBootstrapper,
            GetAppliedStyleBuilder(propertyProvider, styleProvider),
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
                    typeInferrer, propertyProvider, appliedStyleBuilder),
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
                                                                 IPropertyProvider propertyProvider,
                                                                 IAppliedStyleBuilder styleBuilder)
        {
            return new DefaultVisualBootstrapper(resolver, themeProvider, propertyProvider,
                new LayoutQueue(), styleBuilder);
        }

        private static IAppliedStyleBuilder GetAppliedStyleBuilder(IPropertyProvider propertyProvider,
                                                                   IVisualStyleProvider styleProvider)
        {
            //var variableAccessor = new StyleVariableAccessor(themeProvider.ColorPalette);

            //var styleInflater = new DefaultStyleInflater(typeInferrer, variableAccessor);
            //var styleProvider = new VisualStyleProvider(styleInflater, themeProvider,
            //    new BasePrimitiveStyle(variableAccessor));
            //    //new ConcurrentDictionary<Type, IEnumerable<IStyleRule>>());

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
                //new ConcurrentDictionary<Type, IEnumerable<IStyleRule>>());
                new BasePrimitiveStyle(variableAccessor));
            return styleProvider;
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
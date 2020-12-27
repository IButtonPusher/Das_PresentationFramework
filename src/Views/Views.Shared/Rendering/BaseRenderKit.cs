using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Container;
using Das.Serializer;
using Das.Views.Construction;
using Das.Views.Controls;
using Das.Views.Core.Geometry;
using Das.Views.Styles;
using Das.Views.Styles.Construction;
using Das.Views.Templates;

namespace Das.Views
{
    public abstract class BaseRenderKit : IVisualSurrogateProvider
    {
        protected BaseRenderKit(IStyleContext styleContext,
                                IStringPrimitiveScanner attributeScanner,
                                ITypeInferrer typeInferrer,
                                IPropertyProvider propertyProvider,
                                Dictionary<IVisualElement, ValueCube> renderPositions)
            : this(new BaseResolver(), styleContext, attributeScanner,
                typeInferrer, propertyProvider, renderPositions)
        {
        }

        protected BaseRenderKit(IResolver resolver,
                                IStyleContext styleContext,
                                IStringPrimitiveScanner attributeScanner,
                                ITypeInferrer typeInferrer,
                                IPropertyProvider propertyProvider,
                                Dictionary<IVisualElement, ValueCube> renderPositions)
        : this(resolver, styleContext, attributeScanner, typeInferrer, propertyProvider, 
            GetVisualBootstrapper(resolver, styleContext, propertyProvider), renderPositions)
        {
           
        }

        protected BaseRenderKit(IResolver resolver,
                                IStyleContext styleContext,
                                IStringPrimitiveScanner attributeScanner,
                                ITypeInferrer typeInferrer,
                                IPropertyProvider propertyProvider,
                                IVisualBootstrapper visualBootstrapper,
                                Dictionary<IVisualElement, ValueCube> renderPositions)
        : this(resolver, styleContext, visualBootstrapper, 
            GetViewInflater(visualBootstrapper, attributeScanner, 
                typeInferrer, propertyProvider, renderPositions),
            renderPositions)
        
        {

        }
        
        protected BaseRenderKit(IResolver resolver,
                                IStyleContext styleContext,
                                IVisualBootstrapper visualBootstrapper,
                                IViewInflater viewInflater,
                                Dictionary<IVisualElement, ValueCube> renderPositions)
        {
            _styleContext = styleContext;
            _renderPositions = renderPositions;
            Container = resolver;
            
            _surrogateInstances = new Dictionary<IVisualElement, IVisualSurrogate>();
            _surrogateTypeBuilders = new Dictionary<Type, Func<IVisualElement, IVisualSurrogate>>();
            
            VisualBootstrapper = visualBootstrapper;

            ViewInflater = viewInflater;

            resolver.ResolveTo(visualBootstrapper);
        }
        
        private static IVisualBootstrapper GetVisualBootstrapper(IResolver resolver,
                                                                 IStyleContext styleContext,
                                                                 IPropertyProvider propertyProvider)
        {
            return new DefaultVisualBootstrapper(resolver, styleContext, propertyProvider);
        }

        private static IViewInflater GetViewInflater(IVisualBootstrapper visualBootstrapper,
                                                     IStringPrimitiveScanner attributeScanner,
                                                     ITypeInferrer typeInferrer,
                                                     IPropertyProvider propertyProvider,
                                                     Dictionary<IVisualElement, ValueCube> renderPositions)
        {
            var bindingBuilder = new BindingBuilder(typeInferrer, propertyProvider);
            var converterProvider = new DefaultValueConverterProvider(visualBootstrapper);
            var visualTypeResolver = new VisualTypeResolver(typeInferrer);

            var styleInflater = new DefaultStyleInflater(typeInferrer);
            var styleProvider = new VisualStyleProvider(styleInflater);
            var styledVisualBuilder = new StyledVisualBuilder(visualBootstrapper, styleProvider, 
                propertyProvider, renderPositions);
            
            return new ViewInflater(visualBootstrapper, attributeScanner, 
                typeInferrer, bindingBuilder, converterProvider, 
                visualTypeResolver, styledVisualBuilder, propertyProvider);
        }

        public void EnsureSurrogate(ref IVisualElement element)
        {
            if (_surrogateInstances.TryGetValue(element, out var surrogate))
                element = surrogate;
            else if (_surrogateTypeBuilders.TryGetValue(element.GetType(), out var bldr))
            {
                var res = bldr(element);
                _surrogateInstances[element] = res;
                element = res;
            }
        }

        public IResolver Container { get; }

        public IStyleContext StyleContext => _styleContext;

        public IViewInflater ViewInflater { get; }


        public virtual IVisualBootstrapper VisualBootstrapper { get; }

        public virtual void RegisterSurrogate<T>(Func<IVisualElement, IVisualSurrogate> builder)
            where T : IVisualElement
        {
            _surrogateTypeBuilders[typeof(T)] = builder;
        }

        protected readonly IStyleContext _styleContext;
        protected readonly Dictionary<IVisualElement, ValueCube> _renderPositions;
        private readonly Dictionary<IVisualElement, IVisualSurrogate> _surrogateInstances;
        private readonly Dictionary<Type, Func<IVisualElement, IVisualSurrogate>> _surrogateTypeBuilders;
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Container;
using Das.Serializer;
using Das.Views.Construction;
using Das.Views.Controls;
using Das.Views.Styles;
using Das.Views.Templates;

namespace Das.Views
{
    public abstract class BaseRenderKit : IVisualSurrogateProvider
    {
        protected BaseRenderKit(IStyleContext styleContext,
                                IStringPrimitiveScanner attributeScanner,
                                ITypeInferrer typeInferrer,
                                IPropertyProvider propertyProvider)
            : this(new BaseResolver(), styleContext, attributeScanner,
                typeInferrer, propertyProvider)
        {
        }

        protected BaseRenderKit(IResolver resolver,
                                IStyleContext styleContext,
                                IStringPrimitiveScanner attributeScanner,
                                ITypeInferrer typeInferrer,
                                IPropertyProvider propertyProvider)
        {
            _styleContext = styleContext;
            Container = resolver;
            
            _surrogateInstances = new Dictionary<IVisualElement, IVisualSurrogate>();
            _surrogateTypeBuilders = new Dictionary<Type, Func<IVisualElement, IVisualSurrogate>>();
            var templateResolver = new DefaultVisualBootstrapper(resolver, styleContext, propertyProvider);
            VisualBootstrapper = templateResolver;

            var bindingBuilder = new BindingBuilder(typeInferrer, propertyProvider);
            var converterProvider = new DefaultValueConverterProvider();
            
            
            ViewInflater = new ViewInflater(templateResolver, attributeScanner, 
                typeInferrer, bindingBuilder, converterProvider);
            
            resolver.ResolveTo<IVisualBootstrapper>(templateResolver);
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

        public IViewInflater ViewInflater { get; }


        public virtual IVisualBootstrapper VisualBootstrapper { get; }

        public virtual void RegisterSurrogate<T>(Func<IVisualElement, IVisualSurrogate> builder)
            where T : IVisualElement
        {
            _surrogateTypeBuilders[typeof(T)] = builder;
        }

        protected readonly IStyleContext _styleContext;
        private readonly Dictionary<IVisualElement, IVisualSurrogate> _surrogateInstances;
        private readonly Dictionary<Type, Func<IVisualElement, IVisualSurrogate>> _surrogateTypeBuilders;
    }
}
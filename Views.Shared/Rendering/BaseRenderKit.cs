using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Container;
using Das.Views.Controls;
using Das.Views.Rendering;
using Das.Views.Styles;
using Das.Views.Templates;

namespace Das.Views
{
    public abstract class BaseRenderKit : IVisualSurrogateProvider
    {
        protected readonly IStyleContext _styleContext;

        public IResolver Container { get; }

        protected BaseRenderKit(IStyleContext styleContext) 
            : this(new BaseResolver(), styleContext)
        {

        }

        protected BaseRenderKit(IResolver resolver,
                                IStyleContext styleContext)
        {
            _styleContext = styleContext;
            Container = resolver;
            _surrogateInstances = new Dictionary<IVisualElement, IVisualSurrogate>();
            _surrogateTypeBuilders = new Dictionary<Type, Func<IVisualElement,IVisualSurrogate>>();
            var templateResolver = new DefaultVisualBootstrapper(resolver, styleContext);
            VisualBootstrapper = templateResolver;
            resolver.ResolveTo<IVisualBootstrapper>(templateResolver);
        }

        public virtual void RegisterSurrogate<T>(Func<IVisualElement, IVisualSurrogate> builder)
            where T : IVisualElement
        {
            _surrogateTypeBuilders[typeof(T)] = builder;
        }

       
        public virtual IVisualBootstrapper VisualBootstrapper { get; }

        private readonly Dictionary<Type, Func<IVisualElement, IVisualSurrogate>> _surrogateTypeBuilders;
        private readonly Dictionary<IVisualElement, IVisualSurrogate> _surrogateInstances;

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
    }
}
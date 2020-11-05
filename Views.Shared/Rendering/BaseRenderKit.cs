using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Container;
using Das.Views.Controls;
using Das.Views.Rendering;

namespace Das.Views
{
    public abstract class BaseRenderKit : IVisualSurrogateProvider
    {
        public IResolver Resolver { get; }

        protected BaseRenderKit() : this(new BaseResolver())
        {

        }

        protected BaseRenderKit(IResolver resolver)
        {
            Resolver = resolver;
            //_lockContained = new Object();
            _surrogateInstances = new Dictionary<IVisualElement, IVisualSurrogate>();
            _surrogateTypeBuilders = new Dictionary<Type, Func<IVisualElement,IVisualSurrogate>>();
            //_containedObjects = new Dictionary<Type, Object>();
        }

        //public virtual T Resolve<T>()
        //{
        //    return Resolve<T, T>();
        //}

        public virtual void RegisterSurrogate<T>(Func<IVisualElement, IVisualSurrogate> builder)
            where T : IVisualElement
        {
            _surrogateTypeBuilders[typeof(T)] = builder;
        }

        //public virtual TInterface Resolve<TObject, TInterface>()
        //    where TObject : TInterface
        //{
        //    var typeI = typeof(TInterface);

        //    lock (_lockContained)
        //    {
        //        if (_containedObjects.TryGetValue(typeI, out var found))
        //            return (TInterface) found;

        //        var typeO = typeof(TObject);
        //        var ctors = typeO.GetConstructors();
        //        if (ctors.Length != 1)
        //            throw new InvalidOperationException($"{typeO} must have exactly one accessible constructor");

        //        var ctor = ctors[0];
        //        var prms = ctor.GetParameters();

        //        var args = new Object[prms.Length];

        //        for (var c = 0; c < prms.Length; c++)
        //        {
        //            if (!_containedObjects.TryGetValue(prms[c].ParameterType, out var val))
        //                throw new Exception($"Cannot resolve ctor parameter of type {prms[c].ParameterType}");

        //            args[c] = val;
        //        }

        //        var res = ctor.Invoke(args);

        //        _containedObjects[typeI] = res;

        //        return (TInterface) res;
        //    }
        //}

        //public virtual void ResolveTo<TInterface, TObject>(TObject obj)
        //    where TObject : class, TInterface
        //{
        //    lock (_lockContained)
        //    {
        //        _containedObjects[typeof(TInterface)] = obj;
        //    }
        //}

        //protected readonly Dictionary<Type, Object> _containedObjects;
        //private readonly Object _lockContained;
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
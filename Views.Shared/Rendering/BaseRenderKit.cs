using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views
{
    public abstract class BaseRenderKit
    {
        protected BaseRenderKit()
        {
            _lockContained = new Object();
            _containedObjects = new Dictionary<Type, Object>();
        }

        public virtual T Resolve<T>()
        {
            return Resolve<T, T>();
        }

        public virtual TInterface Resolve<TObject, TInterface>()
            where TObject : TInterface
        {
            var typeI = typeof(TInterface);

            lock (_lockContained)
            {
                if (_containedObjects.TryGetValue(typeI, out var found))
                    return (TInterface) found;

                var typeO = typeof(TObject);
                var ctors = typeO.GetConstructors();
                if (ctors.Length != 1)
                    throw new InvalidOperationException($"{typeO} must have exactly one accessible constructor");

                var ctor = ctors[0];
                var prms = ctor.GetParameters();

                var args = new Object[prms.Length];

                for (var c = 0; c < prms.Length; c++)
                {
                    if (!_containedObjects.TryGetValue(prms[c].ParameterType, out var val))
                        throw new Exception($"Cannot resolve ctor parameter of type {prms[c].ParameterType}");

                    args[c] = val;
                }

                var res = ctor.Invoke(args);

                _containedObjects[typeI] = res;

                return (TInterface) res;
            }
        }

        public virtual void ResolveTo<TInterface, TObject>(TObject obj)
            where TObject : class, TInterface
        {
            lock (_lockContained)
            {
                _containedObjects[typeof(TInterface)] = obj;
            }
        }

        protected readonly Dictionary<Type, Object> _containedObjects;
        private readonly Object _lockContained;
    }
}
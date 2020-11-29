using System;
using System.Collections.Generic;
using System.Reflection;
using Das.Container;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Templates
{
    public class DefaultVisualBootstrapper : IVisualBootstrapper
    {
        private readonly IResolver _dependencyResolver;
        private IUiProvider? _uiProvider;
        private readonly Dictionary<Type, ConstructorInfo> _bindingConstructors;
        private readonly Dictionary<Type, ConstructorInfo> _defaultConstructors;

        private readonly Object _bindingConstructorLock;
        private readonly Object _defaultConstructorLock;


        public DefaultVisualBootstrapper(IResolver dependencyResolver,
                                         IStyleContext styleContext)
        {
            StyleContext = styleContext;
            _dependencyResolver = dependencyResolver;

            _bindingConstructorLock = new Object();
            _defaultConstructorLock = new Object();

            _bindingConstructors = new Dictionary<Type, ConstructorInfo>();
            _defaultConstructors = new Dictionary<Type, ConstructorInfo>();
        }

        public void ResolveTo<TViewModel, TView>() 
            where TView : IView<TViewModel>
        {
            throw new NotImplementedException();
        }

        public IDataTemplate? TryResolveFromContext(Object dataContext)
        {
            if (_dependencyResolver.TryResolve<IDataTemplate>(dataContext.GetType(),
                out var dataTemplate))
            {
                return dataTemplate;
            }

            return null;
        }

        public IVisualElement Instantiate(Type type)
        {
            throw new NotImplementedException();
        }

        public IStyleContext StyleContext { get; }

        public TVisualElement Instantiate<TVisualElement>(Type type) 
            where TVisualElement : IVisualElement
        {
            ConstructorInfo? ctor;

            lock (_defaultConstructorLock)
            {
                if (!_defaultConstructors.TryGetValue(type, out ctor))
                {
                    foreach (var ctorMaybe in type.GetConstructors())
                    {
                        var ctorParams = ctorMaybe.GetParameters();
                        if (ctorParams.Length != 1 ||
                            !ctorParams[0].ParameterType.IsAssignableFrom(typeof(IVisualBootstrapper))
                        )
                        {
                            continue;
                        }

                        ctor = ctorMaybe;
                        _defaultConstructors.Add(type, ctorMaybe);
                        break;
                    }
                }
            }

            if (ctor == null)
                throw new MissingMethodException(type.Namespace, "constructor");
            var res = (TVisualElement)ctor.Invoke(new Object[] {this});
            return res;
        }

        //public TBindableElement Instantiate<TBindableElement>(Type type, 
        //                                                      IDataBinding? binding)
        //    where TBindableElement : IBindableElement
        //{
        //    ConstructorInfo? ctor;

        //    lock (_bindingConstructorLock)
        //    {
        //        if (!_bindingConstructors.TryGetValue(type, out ctor))
        //        {
        //            var bindingType = binding != null ? binding.GetType() : typeof(IDataBinding);

        //            foreach (var ctorMaybe in type.GetConstructors())
        //            {
        //                var ctorParams = ctorMaybe.GetParameters();
        //                if (ctorParams.Length != 2 ||
        //                    !ctorParams[0].ParameterType.IsAssignableFrom(bindingType) ||
        //                    !ctorParams[1].ParameterType.IsAssignableFrom(typeof(IVisualBootstrapper))
        //                    )
        //                {
        //                    continue;
        //                }

        //                ctor = ctorMaybe;
        //                _bindingConstructors.Add(type, ctorMaybe);
        //                break;
        //            }
        //        }
        //    }

        //    if (ctor == null)
        //        throw new MissingMethodException(type.Namespace, "constructor");
        //    var res = (TBindableElement)ctor.Invoke(new Object?[] {binding, this});
        //    return res;
        //}


        //public TVisualElement Instantiate<TVisualElement>() 
        //    where TVisualElement : IVisualElement
        //{
        //    throw new NotImplementedException();
        //}

        //public IBindableElement Instantiate(Type type, 
        //                                    Object dataContext)
        //{
        //    throw new NotImplementedException();
        //}

        public TVisualElement InstantiateCopy<TVisualElement>(TVisualElement visual, 
                                                              Object? dataContext) 
            where TVisualElement : IVisualElement
        {
            var copy = (TVisualElement)visual.DeepCopy();
            if (dataContext != null && copy is IBindableElement bindable)
                bindable.DataContext = dataContext;

            return copy;
        }

        public IUiProvider UiProvider => _uiProvider ??=  _dependencyResolver.Resolve<IUiProvider>();
    }
}

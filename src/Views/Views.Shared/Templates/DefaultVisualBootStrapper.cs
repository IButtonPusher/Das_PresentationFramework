using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Das.Container;
using Das.Serializer;
using Das.Serializer.Properties;
using Das.Views.Colors;
using Das.Views.Core.Drawing;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Styles.Construction;

namespace Das.Views.Templates
{
    public class DefaultVisualBootstrapper : IVisualBootstrapper
    {
        public DefaultVisualBootstrapper(IResolver dependencyResolver,
                                         IThemeProvider themeProvider,
                                         IPropertyProvider propertyProvider,
                                         ILayoutQueue layoutQueue,
                                         IAppliedStyleBuilder styleBuilder)
        {
            _dependencyResolver = dependencyResolver;
            _themeProvider = themeProvider;
            _propertyProvider = propertyProvider;
            _styleBuilder = styleBuilder;
            LayoutQueue = layoutQueue;

            _defaultConstructorLock = new Object();
            _defaultConstructors = new Dictionary<Type, ConstructorInfo>();

            
        }

        //public void ResolveTo<TViewModel, TView>()
        //    where TView : IView
        //{
        //    _dependencyResolver.ResolveTo<TViewModel, TView>();
        //    //throw new NotImplementedException();
        //}

        public IDataTemplate? TryResolveFromContext(Object dataContext)
        {
            return _dependencyResolver.TryResolve<IDataTemplate>(dataContext.GetType(),
                out var dataTemplate)
                ? dataTemplate
                : null;
        }

        public Object Resolve(Type type)
        {
            return _dependencyResolver.Resolve(type);
        }

        public T Resolve<T>()
        {
            return _dependencyResolver.Resolve<T>();
        }

        public void ApplyVisualStyling(IVisualElement visual)
        {
            _styleBuilder.ApplyVisualCoreStyles(visual, this);
        }

        public ILayoutQueue LayoutQueue { get; }

        public IVisualElement Instantiate(Type type)
        {
            throw new NotImplementedException();
        }

        public TVisualElement Instantiate<TVisualElement>(Type type)
            where TVisualElement : IVisualElement
        {
            ConstructorInfo? ctor;

            lock (_defaultConstructorLock)
            {
                if (!_defaultConstructors.TryGetValue(type, out ctor))
                    foreach (var ctorMaybe in type.GetConstructors())
                    {
                        var ctorParams = ctorMaybe.GetParameters();
                        if (ctorParams.Length != 1 ||
                            !ctorParams[0].ParameterType.IsAssignableFrom(typeof(IVisualBootstrapper))
                        )
                            continue;

                        ctor = ctorMaybe;
                        _defaultConstructors.Add(type, ctorMaybe);
                        break;
                    }
            }

            if (ctor == null)
                throw new MissingMethodException(type.FullName, "constructor");
            var res = (TVisualElement) ctor.Invoke(new Object[] {this});
            //_styleBuilder.ApplyVisualCoreStyles(res, this);
            return res;
        }

        public TVisualElement Instantiate<TVisualElement>() where TVisualElement : IVisualElement
        {
            return Instantiate<TVisualElement>(typeof(TVisualElement));
        }

        public IBindableElement InstantiateCopy<TVisualElement>(TVisualElement visual,
                                                                Object? dataContext)
            where TVisualElement : IBindableElement
        {
            var obj = InstantiateCopyBase(visual);

            if (dataContext != null)
                throw new NotImplementedException();
            //if (dataContext != null && obj is IBindableElement bindable)
            //    bindable.DataContext = dataContext;


            return obj;
        }

        public TVisualElement InstantiateCopy<TVisualElement>(TVisualElement visual)
            where TVisualElement : IVisualElement
        {
            return InstantiateCopyBase(visual);
        }

        public IVisualElement InstantiateCopy(IVisualElement visual,
                                              Object? dataContext)
        {
            var obj = InstantiateCopyBase(visual);
            if (obj is IBindableElement bindable)
                bindable.DataContext = dataContext;

            return obj;
        }

        public IColorPalette ColorPalette => _themeProvider.ColorPalette;

        public IUiProvider UiProvider => _uiProvider ??= _dependencyResolver.Resolve<IUiProvider>();


        public IPropertyAccessor GetPropertyAccessor(Type declaringType,
                                                     String propertyName)
        {
            return _propertyProvider.GetPropertyAccessor(declaringType, propertyName);
        }

        public IPropertyAccessor<T> GetPropertyAccessor<T>(String propertyName)
        {
            return _propertyProvider.GetPropertyAccessor<T>(propertyName);
        }

        private void CopyChildren(IPanelElement fromPanel,
                                  IPanelElement toPanel)
        {
            foreach (var copyMe in fromPanel.Children.GetAllChildren())
            {
                var iAmCopy = InstantiateCopyBase(copyMe);
                toPanel.AddChild(iAmCopy);
            }
        }

        private void CopyContent(IContentContainer fromPanel,
                                 IContentContainer toPanel)
        {
            if (fromPanel.Content == null)
            {
                toPanel.Content = null;
                return;
            }

            var iAmCopy = InstantiateCopyBase(fromPanel.Content);
            toPanel.Content = iAmCopy;
        }

        private static void CopyDependencyProperties(IVisualElement fromVisual,
                                                     IVisualElement toVisual)
        {
            foreach (var dp in DependencyProperty.GetDependencyPropertiesForType(fromVisual.GetType()))
            {
                var val = dp.GetValue(fromVisual);
                if (val == null)
                    continue;

                dp.SetValueNoTransitions(toVisual, val);
            }
        }

        private TVisualElement InstantiateCopyBase<TVisualElement>(TVisualElement visual)
            where TVisualElement : IVisualElement
        {
            var obj = Instantiate<TVisualElement>(visual.GetType());

            RunOnBoth<IPanelElement>(visual, obj, CopyChildren);

            RunOnBoth<IContentVisual>(visual, obj, CopyContent);

            RunOnBoth<IContentPresenter>(visual, obj, (o,
                                                       c) =>
                c.ContentTemplate = o.ContentTemplate);

            RunOnBoth<IVisualElement>(visual, obj, CopyDependencyProperties);

            RunOnBoth<IBindableElement>(visual, obj, (o,
                                                      c) =>
            {
                foreach (var binding in o.GetBindings())
                {
                    c.AddBinding(binding);
                }
            });

            return obj;
        }

        private static void RunOnBoth<TVisual>(IVisualElement original,
                                               IVisualElement copy,
                                               Action<TVisual, TVisual> action)
            where TVisual : IVisualElement
        {
            if (!(original is TVisual vOriginal) || !(copy is TVisual vCopy))
                return;

            action(vOriginal, vCopy);
        }

        private readonly Object _defaultConstructorLock;
        private readonly Dictionary<Type, ConstructorInfo> _defaultConstructors;
        private readonly IResolver _dependencyResolver;
        private readonly IPropertyProvider _propertyProvider;
        private readonly IAppliedStyleBuilder _styleBuilder;
        private readonly IThemeProvider _themeProvider;

        private IUiProvider? _uiProvider;
    }
}

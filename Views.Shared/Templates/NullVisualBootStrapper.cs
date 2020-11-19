using System;
using System.Collections.Concurrent;
using System.Reflection;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Templates
{
    public class NullVisualBootStrapper : IVisualBootStrapper
    {
        private NullVisualBootStrapper()
        {
            _ctorValue = new Object[] {this};
        }

        public static readonly NullVisualBootStrapper Instance = new NullVisualBootStrapper();
        private readonly Object[] _ctorValue;

        public void ResolveTo<TViewModel, TView>() where TView : IView<TViewModel>
        {
         
        }

        public IView? TryResolve(Object value)
        {
            return null;
        }

        public IVisualElement Instantiate(Type type,
                                          Int32 styleId)
        {
            throw new NotImplementedException();
        }

        IStyleContext IVisualBootStrapper.StyleContext => _styleContext;

        private static Type[] _ctorArgTypes = new Type[] {typeof(IVisualBootStrapper)};

        private static ConcurrentDictionary<Type, ConstructorInfo> _cachedConstructors
            = new ConcurrentDictionary<Type, ConstructorInfo>();

        private static readonly IStyleContext _styleContext =
            new BaseStyleContext(new DefaultStyle(), new DefaultColorPalette());

        public TVisualElement Instantiate<TVisualElement>(Type type) 
            where TVisualElement : IVisualElement
        {
            var ctor = _cachedConstructors.GetOrAdd(type, t => GetConstructor(t));

            return (TVisualElement) ctor.Invoke(_ctorValue);

            
        }

        private static ConstructorInfo GetConstructor(Type type)
        {
            return type.GetConstructor(_ctorArgTypes) ?? throw new MissingMethodException();
        }

        public TVisualElement Instantiate<TVisualElement>() 
            where TVisualElement : IVisualElement
        {
            throw new NotImplementedException();
        }

        public IBindableElement Instantiate(Type type, 
                                            Object dataContext)
        {
            throw new NotImplementedException();
        }
    }
}

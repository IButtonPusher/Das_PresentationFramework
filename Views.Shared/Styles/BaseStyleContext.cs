using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public class BaseStyleContext : IStyleContext
    {
        public IColorPalette ColorPalette { get; }

        static BaseStyleContext()
        {
            _setterTypes = new Dictionary<StyleSetter, Type>();
            _setterCrossInheritance = new Dictionary<StyleSetter, Boolean>();

            var enumType = typeof(StyleSetter);

            foreach (var setter in Enum.GetValues(enumType).OfType<StyleSetter>())
            {
                var memberInfos = enumType.GetMember(setter.ToString());
                var enumValueMemberInfo = memberInfos.First(m => m.DeclaringType == enumType);
                var valueAttributes = enumValueMemberInfo.
                    GetCustomAttributes(typeof(StyleTypeAttribute), false);

                var attr = (StyleTypeAttribute)valueAttributes.First(a => a is StyleTypeAttribute);
                _setterTypes.Add(setter, attr.Type);
                _setterCrossInheritance.Add(setter, attr.IsCrossTypeInheritable);
            }
        }

        public BaseStyleContext(IStyle defaultStyle,
                                IColorPalette colorPalette)
        {
            ColorPalette = colorPalette;
            AssertStyleValidity(defaultStyle, true);
            _defaultStyle = defaultStyle;

            _visualStack = new Stack<IVisualElement>();
            _visualSearchStack = new Stack<IVisualElement>();

            _accentColor = colorPalette.Accent;

            _elementStyles = new Dictionary<Int32, ElementStyle>();
            _cachedStyles = new Dictionary<Int32, IStyle>();
            _typeStyles = new Dictionary<Type, List<ScopedStyle>>();

            if (defaultStyle is IStyleSheet sheet)
                foreach (var kvp in sheet.VisualTypeStyles)
                    RegisterStyle(kvp.Value);
        }

        public virtual IEnumerable<IStyle> GetStylesForElement(IVisualElement element)
        {
            if (_elementStyles.TryGetValue(element.Id, out var elementStyles))
            {
                yield return elementStyles;
                //foreach (var rdrr in elementStyles)
                //    yield return rdrr;
            }

            if (_cachedStyles.TryGetValue(element.Id, out var known))
            {
                yield return known;
                yield break;
            }

            element.Disposed += OnElementDisposed;

            var buildCached = BuildCachedStyle(element);
            
            
            _cachedStyles[element.Id] = buildCached;
            yield return buildCached;
        }

        public void RegisterStyle(IStyle style)
        {
            RegisterStyleImpl(style, null);
        }

        public void RegisterStyle(IStyle style,
                                  IVisualElement scope)
        {
            RegisterStyleImpl(style, scope);
        }

        public void SetCurrentAccentColor(IColor color)
        {
            _accentColor = color;
        }

        public void RegisterStyleSetter(IVisualElement element,
                                        StyleSetter setter,
                                        Object value)
        {
            if (!IsTypeValid(setter, value))
                throw new ArgumentOutOfRangeException(nameof(value));

            if (!_elementStyles.TryGetValue(element.Id, out var style))
            {
                style = new ElementStyle(element, this);
                _elementStyles[element.Id] = style;
            }

            //var style = new ElementStyle(element);
            style.AddSetter(setter, value);
           // RegisterStyle(style, element);
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetter setter, 
                                        StyleSelector selector, 
                                        Object value)
        {
            if (!IsTypeValid(setter, value))
                throw new ArgumentOutOfRangeException(nameof(value));

            if (!_elementStyles.TryGetValue(element.Id, out var style))
            {
                style = new ElementStyle(element, this);
                _elementStyles[element.Id] = style;
            }

            
            style.Add(setter, selector, value);
            
        }

        public IColor GetCurrentAccentColor()
        {
            return _accentColor;
        }

        public void RegisterStyleSetter<T>(StyleSetter setter,
                                           Object value,
                                           IVisualElement scope)
            where T : IVisualElement
        {
            var style = new TypeStyle<T>();
            style[setter] = value;
            RegisterStyle(style, scope);
        }

        public void CoerceIsChanged()
        {
            IsChanged = true;
        }

        public void PushVisual(IVisualElement visual)
        {
            _visualStack.Push(visual);
        }

        public IVisualElement PopVisual()
        {
            return _visualStack.Pop();
        }

        public T GetStyleSetter<T>(StyleSetter setter,
                                   IVisualElement element)
        {
            return GetStyleSetter<T>(setter, StyleSelector.None, element);
        }


        public T GetStyleSetter<T>(StyleSetter setter,
                                   StyleSelector selector,
                                   IVisualElement element)
        {
            if (TryGetStyleSetterImpl<T>(setter, selector, element, out var found))
                return found;

            if (selector != StyleSelector.None &&
                TryGetStyleSetterImpl(setter, StyleSelector.None, element, out found))
                return found;

            return _defaultStyle[setter] is T good ? good : default!;

            //var res = GetStyleSetterImpl<T>(setter, selector, element);
            //if (res != null || selector == StyleSelector.None)
            //    return res;

            //return GetStyleSetterImpl<T>(setter, StyleSelector.None, element);
        }


        //private T GetStyleSetterImpl<T>(StyleSetter setter,
        //                           StyleSelector selector,
        //                           IVisualElement element)
        //{
        //    if (TryGetStyleSetterImpl<T>(setter, selector, element, out var found))
        //        return found;

        //    if (selector != StyleSelector.None &&
        //        TryGetStyleSetterImpl(setter, StyleSelector.None, element, out found))
        //        return found;

        //    return _defaultStyle[setter] is T good ? good : default!;
        //}

        private Boolean TryGetStyleSetterForElement<T>(StyleSetter setter,
                                                 StyleSelector selector,
                                                 IVisualElement element,
                                                 out T found)
        {
            if (_elementStyles.TryGetValue(element.Id, out var elementStyle) &&
                TryGetStyleSetterImpl(setter, elementStyle, selector, element, out found))
                return true;

            if (_cachedStyles.TryGetValue(element.Id, out var known) && 
                TryGetStyleSetterImpl(setter, known, selector, element, out found))
                return true;

            found = default!;
            return false;
        }

        private Boolean TryGetStyleSetterImpl<T>(StyleSetter setter,
                                                 StyleSelector selector,
                                                 IVisualElement element,
                                                 out T found)
        {
            if (TryGetStyleSetterForElement(setter, selector, element, out found))
                return true;

            try
            {
                var isCrossInheritable = _setterCrossInheritance[setter];

                var allMyScoped = new Dictionary<IVisualElement, IStyle>();
                    
                    foreach (var kvp in GetAllScopedStyles(element))
                    {
                        if (kvp.Scope == null)
                        {
                            if (TryGetStyleSetterImpl(setter, kvp.Style,
                                selector, element, out found))
                            {
                                return true;
                            }
                        
                            continue;
                        }

                        allMyScoped[kvp.Scope] = kvp.Style;
                    }

                while (_visualStack.Count > 0)
                {
                    var current = _visualStack.Pop();
                    _visualSearchStack.Push(current);

                    if (allMyScoped.TryGetValue(current, out var scoped) &&
                        TryGetStyleSetterImpl(setter, scoped, selector, element, out found))
                        return true;

                    if (isCrossInheritable && 
                        TryGetStyleSetterForElement(setter, selector, current, out found))
                        return true;
                }

                if (_defaultStyle[setter] is T good)
                {
                    found = good;
                    return true;
                }

                found = default!;
                return false;
            }
            finally
            {
                if (!_cachedStyles.TryGetValue(element.Id, out var cachedStyle))
                {
                    cachedStyle = new Style();
                    _cachedStyles[element.Id] = cachedStyle;
                }

                cachedStyle.Add(setter, selector, found);

                while (_visualSearchStack.Count > 0)
                {
                    var current = _visualSearchStack.Pop();
                    _visualStack.Push(current);
                }
            }

            //var styles = GetStylesForElement(element);
            //foreach (var style in styles)
            //{
            //    Object? v = null;

            //    if (asDc == null && !style.TryGetValue(setter, selector, out v))
            //        continue;
            //    if (asDc != null && !style.TryGetValue(setter, selector, asDc, out v))
            //        continue;

            //    switch (v)
            //    {
            //        case T val:
            //            found = val;
            //            return true;

            //        case IConvertible _ when typeof(IConvertible).IsAssignableFrom(typeof(T)) &&
            //                                 Convert.ChangeType(v, typeof(T)) is T cval:
            //            found = cval;
            //            return true;

            //        case IConvertible _ when Convert.ChangeType(v, typeof(Double)) is Double dbl:
            //            //yuck
            //            var ctor = typeof(T).GetConstructor(new[] {typeof(Double)});
            //            if (ctor != null)
            //            {
            //                found =(T) ctor.Invoke(new Object[] {dbl});
            //                return true;
            //            }

            //            break;
            //    }
            //}

            //found = default!;
            //return false;
        }

        private static Boolean TryGetStyleSetterImpl<T>(
            StyleSetter setter,
            IStyle style,
            StyleSelector selector,
            IVisualElement element,
            out T found)
        {
            var asDc = (element as IDataContext)?.Value;

            //foreach (var style in styles)
            {
                Object? v = null;

                if (asDc == null && !style.TryGetValue(setter, selector, out v))
                    goto failBoat;
                if (asDc != null && !style.TryGetValue(setter, selector, asDc, out v))
                    goto failBoat;

                switch (v)
                {
                    case T val:
                        found = val;
                        return true;

                    case IConvertible _ when typeof(IConvertible).IsAssignableFrom(typeof(T)) &&
                                             Convert.ChangeType(v, typeof(T)) is T cval:
                        found = cval;
                        return true;

                    case IConvertible _ when Convert.ChangeType(v, typeof(Double)) is Double dbl:
                        //yuck
                        var ctor = typeof(T).GetConstructor(new[] {typeof(Double)});
                        if (ctor != null)
                        {
                            found = (T) ctor.Invoke(new Object[] {dbl});
                            return true;
                        }

                        break;
                }
            }

            failBoat:

            found = default!;
            return false;
        }

        private static void AssertStyleValidity(IStyle style,
                                                Boolean isRequireExhaustive)
        {
            //foreach (StyleSetter setter in Enum.GetValues(typeof(StyleSetter)))
            foreach (var setter in _setterTypes.Keys)
            {
                if (style.TryGetValue(setter, StyleSelector.None, out var val))
                {
                    if (!IsTypeValid(setter, val))
                        throw new Exception("Setter: " + setter + " is of the wrong type");
                }
                else if (isRequireExhaustive)
                {
                    throw new Exception("Default style is missing a setter for " + setter);
                }
            }
        }

        private IEnumerable<ScopedStyle> GetAllScopedStyles(IVisualElement visual)
        {
            foreach (var type in GetStylableTypes(visual))
            {
                if (!_typeStyles.TryGetValue(type, out var styleCollection))
                    continue;

                foreach (var style in styleCollection)
                    yield return style;
            }
        }

        private Style BuildCachedStyle(IVisualElement element)
        {
            var res = new Style();

            if (_elementStyles.TryGetValue(element.Id, out var styles))
            {
                res.AddMissingSetters(styles);
                //for (var c = styles.Count - 1; c >= 0; c--)
                //    res.AddMissingSetters(styles[c]);
            }

            foreach (var type in GetStylableTypes(element))
            {
                if (!_typeStyles.TryGetValue(type, out var styleCollection)) 
                    continue;

                foreach (var style in styleCollection)
                {
                    switch (style.Scope)
                    {
                        case IVisualFinder container:
                            if (container.Contains(element))
                                res.AddMissingSetters(style.Style);
                            break;

                        case IVisualElement ele when ele == element:
                            res.AddMissingSetters(style.Style);
                            break;

                        case null:
                            res.AddMissingSetters(style.Style);
                            break;
                    }

                    if (styles != default)
                    {
                        res.AddMissingSetters(styles);
                        //for (var c = styles.Count - 1; c >= 0; c--)
                        //    res.AddMissingSetters(styles[c]);
                    }
                }
            }


           

            res.AddMissingSetters(_defaultStyle);

            return res;
        }

        private static IEnumerable<Type> GetStylableTypes(IVisualElement element)
        {
            var returned = new HashSet<Type>();

            var current = element.GetType();

            while (current != null && typeof(IVisualElement).IsAssignableFrom(current))
            {
                if (returned.Add(current))
                    yield return current;

                current = current.BaseType;
            }

            foreach (var impl in element.GetType().GetInterfaces())
            {
                if (typeof(IVisualElement).IsAssignableFrom(impl) &&
                    returned.Add(impl))
                {
                    yield return impl;
                }
            }

        }

        private static Boolean IsTypeValid(StyleSetter setter,
                                           Object value)
        {
            switch (setter)
            {
                case StyleSetter.Margin:
                case StyleSetter.Padding:
                case StyleSetter.BorderThickness:
                    return value is Thickness ||
                           value is IConvertible;
                case StyleSetter.Font:
                    return value is Font;
                case StyleSetter.FontName:
                    return value is String;
                case StyleSetter.FontSize:
                    return value is IConvertible;
                case StyleSetter.FontWeight:
                    return value is FontStyle;
                case StyleSetter.Foreground:
                case StyleSetter.Background:
                case StyleSetter.BorderBrush:
                    return value is SolidColorBrush;
                case StyleSetter.VerticalAlignment:
                    return value is VerticalAlignments;
                case StyleSetter.HorizontalAlignment:
                    return value is HorizontalAlignments;
                //case StyleSetter.Size:
                //    return value == null || value is ISize;
                case StyleSetter.Height:
                case StyleSetter.Width:
                    return value == null || value is IConvertible;

                default:
                    return value.GetType().IsAssignableFrom(_setterTypes[setter]);
                    //var enumType = typeof(StyleSetter);
                    //var memberInfos = enumType.GetMember(setter.ToString());
                    //var enumValueMemberInfo = memberInfos.First(m => m.DeclaringType == enumType);
                    //var valueAttributes =
                    //    enumValueMemberInfo.GetCustomAttributes(typeof(StyleTypeAttribute), false);
                    //var description = ((StyleTypeAttribute) valueAttributes[0]).Type;

                    //return value.GetType().IsAssignableFrom(description);
            }
        }

        private void OnElementDisposed(IVisualElement obj)
        {
            _cachedStyles.Remove(obj.Id);
        }

        private void RegisterStyleImpl(IStyle style,
                                       IVisualElement? scope)
        {
            AssertStyleValidity(style, false);

            switch (style)
            {
                case ElementStyle elementStyle:
                    if (!_elementStyles.TryGetValue(elementStyle.Element.Id, out var forElement))
                    {
                        //forElement = new List<IStyle>();
                        forElement = elementStyle;
                        _elementStyles.Add(elementStyle.Element.Id, forElement);
                        //forElement.Add(elementStyle);
                    }
                    else
                    {
                        forElement.AddOrUpdate(elementStyle);
                        //forElement[0].AddOrUpdate(elementStyle);
                    }

                    //forElement.Add(elementStyle);
                    break;
                case TypeStyle typeStyle:
                    Type? forType = null;
                    var currentType = typeStyle.GetType();
                    while (currentType != typeof(Object) && currentType != null)
                    {
                        forType = currentType.GetGenericArguments().FirstOrDefault();
                        if (forType != null && typeof(IVisualElement).IsAssignableFrom(forType))
                            break;
                        currentType = currentType.BaseType;
                    }

                    if (forType == null)
                        throw new InvalidOperationException("TypeStyle must be of the generic variant");

                    if (!_typeStyles.TryGetValue(forType, out var typeStyles))
                    {
                        typeStyles = new List<ScopedStyle>();
                        _typeStyles.Add(forType, typeStyles);
                    }

                    var scopedStyle = new ScopedStyle(scope, style);
                    typeStyles.Add(scopedStyle);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        //private readonly Dictionary<IVisualElement, IStyle> _cachedStyles;
        private readonly Dictionary<Int32, IStyle> _cachedStyles;

        private readonly IStyle _defaultStyle;
        private IColor _accentColor;
        private readonly Dictionary<Int32, ElementStyle> _elementStyles;
        private readonly Dictionary<Type, List<ScopedStyle>> _typeStyles;

        private readonly Stack<IVisualElement> _visualStack;
        private readonly Stack<IVisualElement> _visualSearchStack;

        private static readonly Dictionary<StyleSetter, Type> _setterTypes;
        private static readonly Dictionary<StyleSetter, Boolean> _setterCrossInheritance;

        public void AcceptChanges()
        {
            IsChanged = false;
        }

        public Boolean IsChanged { get; private set; }
    }
}
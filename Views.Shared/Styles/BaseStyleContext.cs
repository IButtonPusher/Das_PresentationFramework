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
        public BaseStyleContext(IStyle defaultStyle)
        {
            AssertStyleValidity(defaultStyle, true);
            _defaultStyle = defaultStyle;
            _elementStyles = new Dictionary<Int32, List<IStyle>>();
            _cachedStyles = new Dictionary<IVisualElement, IStyle>();
            _typeStyles = new Dictionary<Type, List<ScopedStyle>>();

            if (defaultStyle is IStyleSheet sheet)
                foreach (var kvp in sheet.VisualTypeStyles)
                    RegisterStyle(kvp.Value);
        }

        public virtual IEnumerable<IStyle> GetStylesForElement(IVisualElement element)
        {
            if (_cachedStyles.TryGetValue(element, out var known))
            {
                yield return known;
                yield break;
            }

            var buildCached = BuildCachedStyle(element);
            element.Disposed += OnElementDisposed;
            _cachedStyles[element] = buildCached;
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

        public void RegisterStyleSetter(IVisualElement element,
                                        StyleSetter setter,
                                        Object value)
        {
            var style = new ElementStyle(element);
            style.AddSetter(setter, value);
            RegisterStyle(style, element);
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

        private Boolean TryGetStyleSetterImpl<T>(StyleSetter setter,
                                                 StyleSelector selector,
                                                 IVisualElement element,
                                                 out T found)
        {
            var asDc = (element as IDataContext)?.Value;

            var styles = GetStylesForElement(element);
            foreach (var style in styles)
            {
                Object? v = null;

                if (asDc == null && !style.TryGetValue(setter, selector, out v))
                    continue;
                if (asDc != null && !style.TryGetValue(setter, selector, asDc, out v))
                    continue;

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
                            found =(T) ctor.Invoke(new Object[] {dbl});
                            return true;
                        }

                        break;
                }
            }

            found = default!;
            return false;
        }

        private static void AssertStyleValidity(IStyle style,
                                                Boolean isRequireExhaustive)
        {
            foreach (StyleSetter setter in Enum.GetValues(typeof(StyleSetter)))
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

        private Style BuildCachedStyle(IVisualElement element)
        {
            var res = new Style();

            if (_elementStyles.TryGetValue(element.Id, out var styles))
                for (var c = styles.Count - 1; c >= 0; c--)
                    res.AddMissingSetters(styles[c]);

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
                        for (var c = styles.Count - 1; c >= 0; c--)
                            res.AddMissingSetters(styles[c]);
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
                case StyleSetter.Size:
                    return value == null || value is ISize;
                case StyleSetter.Height:
                case StyleSetter.Width:
                    return value == null || value is IConvertible;

                default:
                    var enumType = typeof(StyleSetter);
                    var memberInfos = enumType.GetMember(setter.ToString());
                    var enumValueMemberInfo = memberInfos.First(m => m.DeclaringType == enumType);
                    var valueAttributes =
                        enumValueMemberInfo.GetCustomAttributes(typeof(StyleTypeAttribute), false);
                    var description = ((StyleTypeAttribute) valueAttributes[0]).Type;

                    return value.GetType().IsAssignableFrom(description);
            }
        }

        private void OnElementDisposed(IVisualElement obj)
        {
            _cachedStyles.Remove(obj);
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
                        forElement = new List<IStyle>();
                        _elementStyles.Add(elementStyle.Element.Id, forElement);
                    }

                    forElement.Add(elementStyle);
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

        private readonly Dictionary<IVisualElement, IStyle> _cachedStyles;

        private readonly IStyle _defaultStyle;
        private readonly Dictionary<Int32, List<IStyle>> _elementStyles;
        private readonly Dictionary<Type, List<ScopedStyle>> _typeStyles;
    }
}
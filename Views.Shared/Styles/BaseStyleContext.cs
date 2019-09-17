using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IStyle _defaultStyle;
        private readonly Dictionary<Type, List<ScopedStyle>> _typeStyles;
        private readonly Dictionary<Int32, List<IStyle>> _elementStyles;

        public BaseStyleContext(IStyle defaultStyle)
        {
            AssertStyleValidity(defaultStyle, true);
            _defaultStyle = defaultStyle;
            _elementStyles = new Dictionary<Int32, List<IStyle>>();
            _typeStyles = new Dictionary<Type, List<ScopedStyle>>();
        }

        private static void AssertStyleValidity(IStyle style, Boolean isRequireExhaustive)
        {
            foreach (StyleSetters setter in Enum.GetValues(typeof(StyleSetters)))
            {
                if (style.TryGetValue(setter, out var val))
                {
                    if (!IsTypeValid(setter, val))
                        throw new Exception("Setter: " + setter + " is of the wrong type");
                }
                else if (isRequireExhaustive)
                    throw new Exception("Default style is missing a setter for " + setter);
            }
        }

        private static Boolean IsTypeValid(StyleSetters setter, Object value)
        {
            switch (setter)
            {
                case StyleSetters.Margin:
                case StyleSetters.Padding:
                case StyleSetters.BorderThickness:
                    return value is Thickness ||
                           value is IConvertible;
                case StyleSetters.Font:
                    return value is Font;
                case StyleSetters.FontName:
                    return value is String;
                case StyleSetters.FontSize:
                    return value is IConvertible;
                case StyleSetters.FontWeight:
                    return value is FontStyle;
                case StyleSetters.Foreground:
                case StyleSetters.Background:
                case StyleSetters.BorderBrush:
                    return value is Brush;
                case StyleSetters.VerticalAlignment:
                    return value is VerticalAlignments;
                case StyleSetters.HorizontalAlignment:
                    return value is HorizontalAlignments;
                case StyleSetters.Size:
                    return value == null || value is ISize;
                case StyleSetters.Height:
                case StyleSetters.Width:
                    return value == null || value is IConvertible;
                default:
                    throw new ArgumentOutOfRangeException(nameof(setter), setter, null);
            }
        }

        public virtual IEnumerable<IStyle> GetStylesForElement(IVisualElement element)
        {
            if (_elementStyles.TryGetValue(element.Id, out var styles))
            {
                for (var c = styles.Count - 1; c >= 0; c--)
                    yield return styles[c];
            }

            if (_typeStyles.TryGetValue(element.GetType(), out var styleCollection))
            {
                foreach (var style in styleCollection)
                {
                    switch (style.Scope)
                    {
                        case IVisualFinder container:
                            if (container.Contains(element))
                            {
                                yield return style.Style;
                            }
                            break;
                        case IVisualElement ele when ele == element:
                            yield return style.Style;
                            break;
                    }

                    if (styles != default)
                    {
                        for (var c = styles.Count - 1; c >= 0; c--)
                            yield return styles[c];
                    }
                }
            }

            yield return _defaultStyle;
        }

        public void RegisterStyle(IStyle style, IVisualElement scope)
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
                    Type forType = null;
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

        public void RegisterStyleSetter(IVisualElement element, StyleSetters setter, Object value)
        {
            var style = new ElementStyle(element);
            style.Setters[setter] = value;
            RegisterStyle(style, element);
        }

        public void RegisterStyleSetter<T>(StyleSetters setter, Object value,
            IVisualElement scope) 
            where T : IVisualElement
        {
            var style = new TypeStyle<T>();
            style.Setters[setter] = value;
            RegisterStyle(style, scope);
        }

        public T GetStyleSetter<T>(StyleSetters setter, IVisualElement element)
        {
            var asDc = (element as IDataContext)?.Value;

            var styles = GetStylesForElement(element);
            foreach (var style in styles)
            {
                Object v = null;

                if (asDc == null && !style.TryGetValue(setter, out v))
                    continue;
                if (asDc != null && !style.TryGetValue(setter, asDc, out v))
                    continue;

                switch (v)
                {
                    case T val:
                        return val;
                    case IConvertible _ when typeof(IConvertible).IsAssignableFrom(typeof(T)) &&
                                             Convert.ChangeType(v, typeof(T)) is T cval:
                        return cval;
                    case IConvertible _ when Convert.ChangeType(v, typeof(Double)) is Double dbl:
                        //yuck
                        var ctor = typeof(T).GetConstructor(new[] {typeof(Double)});
                        if (ctor != null)
                            return (T) ctor.Invoke(new Object[] {dbl});
                        break;
                }
            }

            return _defaultStyle[setter] is T good ? good : default;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public class BaseStyleContext : IStyleContext
    {
        static BaseStyleContext()
        {
            _setterTypes = new Dictionary<StyleSetterType, Type>();
            _setterCrossInheritance = new Dictionary<StyleSetterType, Boolean>();

            var enumType = typeof(StyleSetterType);

            foreach (var setter in Enum.GetValues(enumType).OfType<StyleSetterType>())
            {
                var memberInfos = enumType.GetMember(setter.ToString());
                var enumValueMemberInfo = memberInfos.First(m => m.DeclaringType == enumType);
                var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(StyleTypeAttribute), false);

                var attr = (StyleTypeAttribute) valueAttributes.First(a => a is StyleTypeAttribute);
                _setterTypes.Add(setter, attr.Type);
                _setterCrossInheritance.Add(setter, attr.IsCrossTypeInheritable);
            }
        }

        public BaseStyleContext(IStyle defaultStyle,
                                IColorPalette colorPalette,
                                IStyleVariableAccessor variableAccessor)
        {
            ColorPalette = colorPalette;
            AssertStyleValidity(defaultStyle, true);
            _defaultStyle = defaultStyle;
            _variableAccessor = variableAccessor;

            _elementStyles = new Dictionary<Int32, ElementStyle>();
            _cachedStyles = new Dictionary<Int32, IStyle>();
            _typeStyles = new Dictionary<Type, List<ScopedStyle>>();

            if (defaultStyle is IStyleSheet sheet)
                //throw new NotImplementedException();
                foreach (var kvp in sheet.VisualTypeStyles)
                    RegisterStyle(kvp.Value);
            
            RegisterMaterialColors(colorPalette, variableAccessor);
        }

        private static void RegisterMaterialColors(IColorPalette colorPalette,
                                                   IStyleVariableAccessor variableAccessor)
        {
            variableAccessor.SetVariableValue("--pure-material-onsurface-rgb",
                () => colorPalette.OnSurface);
        }

        public IColorPalette ColorPalette { get; }

        public virtual IEnumerable<IStyle> GetStylesForElement(IVisualElement element)
        {
            if (_elementStyles.TryGetValue(element.Id, out var elementStyles))
                yield return elementStyles;

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

        public void RegisterStyleSetter(IVisualElement element,
                                        StyleSetterType setterType,
                                        Object value)
        {
            if (!IsTypeValid(setterType, value))
                throw new ArgumentOutOfRangeException(nameof(value));

            if (!_elementStyles.TryGetValue(element.Id, out var style))
            {
                style = new ElementStyle(element, this);
                _elementStyles[element.Id] = style;
            }

            style.AddSetter(setterType, value);
        }

        public void RegisterStyleSetter(IVisualElement element,
                                        StyleSetterType setterType,
                                        VisualStateType type,
                                        Object value)
        {
            if (!IsTypeValid(setterType, value))
                throw new ArgumentOutOfRangeException(nameof(value));

            if (!_elementStyles.TryGetValue(element.Id, out var style))
            {
                style = new ElementStyle(element, this);
                _elementStyles[element.Id] = style;
            }


            style.Add(setterType, type, value);
        }

        public void RegisterStyleSetter<T>(StyleSetterType setterType,
                                           Object value,
                                           IVisualElement scope)
            where T : IVisualElement
        {
            var style = new TypeStyle<T>();
            style[setterType] = value;
            RegisterStyle(style, scope);
        }

        public void CoerceIsChanged()
        {
            IsChanged = true;
        }

        public IStyleVariableAccessor VariableAccessor => _variableAccessor;

        public T GetStyleSetter<T>(StyleSetterType setterType,
                                   IVisualElement element,
                                   IVisualLineage visualLineage)
        {
            return GetStyleSetter<T>(setterType, VisualStateType.None,
                element, visualLineage);
        }


        public T GetStyleSetter<T>(StyleSetterType setterType,
                                   VisualStateType type,
                                   IVisualElement element,
                                   IVisualLineage visualLineage)
        {
            if (TryGetStyleSetterImpl<T>(setterType, type, element, visualLineage, out var found))
                return found;

            if (type != VisualStateType.None &&
                TryGetStyleSetterImpl(setterType, VisualStateType.None,
                    element, visualLineage, out found))
                return found;

            return _defaultStyle[setterType] is T good ? good : default!;
        }

        public void AcceptChanges()
        {
            IsChanged = false;
        }

        public Boolean IsChanged { get; private set; }

        private static void AssertStyleValidity(IStyle style,
                                                // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
                                                Boolean isRequireExhaustive)
        {
            //foreach (StyleSetter setter in Enum.GetValues(typeof(StyleSetter)))
            foreach (var setter in _setterTypes.Keys)
                if (style.TryGetValue(setter, VisualStateType.None, out var val))
                {
                    if (!IsTypeValid(setter, val))
                        throw new Exception("Setter: " + setter + " is of the wrong type");
                }
                else if (isRequireExhaustive) throw new Exception("Default style is missing a setter for " + setter);
        }

        private Style BuildCachedStyle(IVisualElement element)
        {
            var res = new Style();

            if (_elementStyles.TryGetValue(element.Id, out var styles))
                res.AddMissingSetters(styles);
            //for (var c = styles.Count - 1; c >= 0; c--)
            //    res.AddMissingSetters(styles[c]);

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
                        res.AddMissingSetters(styles);
                    //for (var c = styles.Count - 1; c >= 0; c--)
                    //    res.AddMissingSetters(styles[c]);
                }
            }


            res.AddMissingSetters(_defaultStyle);

            return res;
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
                if (typeof(IVisualElement).IsAssignableFrom(impl) &&
                    returned.Add(impl))
                    yield return impl;
        }

        private static Boolean IsTypeValid(StyleSetterType setterType,
                                           Object? value)
        {
            switch (setterType)
            {
                case StyleSetterType.Margin:
                case StyleSetterType.Padding:
                case StyleSetterType.BorderThickness:
                    return value is Thickness ||
                           value is IConvertible;
                case StyleSetterType.Font:
                    return value is Font;
                case StyleSetterType.FontName:
                    return value is String;
                case StyleSetterType.FontSize:
                    return value is IConvertible;
                case StyleSetterType.FontWeight:
                    return value is FontStyle;
                case StyleSetterType.Foreground:
                case StyleSetterType.Background:
                case StyleSetterType.BorderBrush:
                    return value is SolidColorBrush;
                case StyleSetterType.VerticalAlignment:
                    return value is VerticalAlignments;
                case StyleSetterType.HorizontalAlignment:
                    return value is HorizontalAlignments;
                //case StyleSetter.Size:
                //    return value == null || value is ISize;
                case StyleSetterType.Height:
                case StyleSetterType.Width:
                    return value == null || value is IConvertible;

                default:
                    return value == null ||
                           value.GetType().IsAssignableFrom(_setterTypes[setterType]);
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
                    
                        forElement = elementStyle;
                        _elementStyles.Add(elementStyle.Element.Id, forElement);
                    }
                    else
                        forElement.AddOrUpdate(elementStyle);
                    
                    break;
                case TypeStyle typeStyle:

                    var forType = typeStyle.TargetType;

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


        private Boolean TryGetStyleSetterForElement<T>(StyleSetterType setterType,
                                                       VisualStateType type,
                                                       IVisualElement element,
                                                       out T found)
        {
            if (_elementStyles.TryGetValue(element.Id, out var elementStyle) &&
                TryGetStyleSetterImpl(setterType, elementStyle, type, out found))
                return true;

            if (_cachedStyles.TryGetValue(element.Id, out var known) &&
                TryGetStyleSetterImpl(setterType, known, type, out found))
                return true;

            found = default!;
            return false;
        }

        private Boolean TryGetStyleSetterImpl<T>(StyleSetterType setterType,
                                                 VisualStateType type,
                                                 IVisualElement element,
                                                 IVisualLineage visualLineage,
                                                 out T found)
        {
            if (TryGetStyleSetterForElement(setterType, type, element, out found))
                return true;

            try
            {
                var isCrossInheritable = _setterCrossInheritance[setterType];

                var allMyScoped = new Dictionary<IVisualElement, IStyle>();

                foreach (var kvp in GetAllScopedStyles(element))
                {
                    if (kvp.Scope == null)
                    {
                        if (TryGetStyleSetterImpl(setterType, kvp.Style,
                            type, out found))
                            return true;

                        continue;
                    }

                    allMyScoped[kvp.Scope] = kvp.Style;
                }

                foreach (var current in visualLineage)
                {
                    if (allMyScoped.TryGetValue(current, out var scoped) &&
                        TryGetStyleSetterImpl(setterType, scoped, type, out found))
                        return true;

                    if (isCrossInheritable &&
                        TryGetStyleSetterForElement(setterType, type, current, out found))
                        return true;
                }

                if (type != VisualStateType.None)
                    // prefer a style setter that ignores the selector
                    // rather than a bare bones default style
                    return TryGetStyleSetterImpl(setterType, VisualStateType.None,
                        element, visualLineage, out found);

                if (_defaultStyle[setterType] is T good)
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

                cachedStyle.Add(setterType, type, found);
            }
        }

        private static Boolean TryGetStyleSetterImpl<T>(
            StyleSetterType setterType,
            IStyle style,
            VisualStateType type,
            out T found)
        {
            if (!style.TryGetValue(setterType, type, out var v))
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


            failBoat:

            found = default!;
            return false;
        }

        private static readonly Dictionary<StyleSetterType, Type> _setterTypes;
        private static readonly Dictionary<StyleSetterType, Boolean> _setterCrossInheritance;

        private readonly Dictionary<Int32, IStyle> _cachedStyles;

        private readonly IStyle _defaultStyle;
        private readonly IStyleVariableAccessor _variableAccessor;
        private readonly Dictionary<Int32, ElementStyle> _elementStyles;
        private readonly Dictionary<Type, List<ScopedStyle>> _typeStyles;
    }
}
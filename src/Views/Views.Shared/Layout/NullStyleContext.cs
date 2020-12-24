using System;
using System.Collections.Generic;
using Das.Views.Colors;
using Das.Views.Core.Drawing;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Layout
{
    public class NullStyleContext : IStyleContext
    {

        IColorPalette IThemeProvider.ColorPalette => ThrowException<IColorPalette>();

        private static T ThrowException<T>()
        {
            throw new NotSupportedException("Style provider wasn't set.");
        }

        public void RegisterStyleSetter(IVisualElement element, StyleSetterType setterType, Object value)
        {
            ThrowException<Object>();
        }

        public void RegisterStyleSetter(IVisualElement element, StyleSetterType setterType, VisualStateType type, Object value)
        {
            ThrowException<Object>();
        }

        public void AcceptChanges()
        {
            ThrowException<Object>();
        }

        public Boolean IsChanged => ThrowException<Boolean>();

        public IEnumerable<IStyle> GetStylesForElement(IVisualElement element)
        {
            return ThrowException<IEnumerable<IStyle>>();
        }

        public void RegisterStyle(IStyle style)
        {
            ThrowException<Object>();
        }

        public void RegisterStyle(IStyle style, IVisualElement scope)
        {
            ThrowException<Object>();
        }

        public T GetStyleSetter<T>(StyleSetterType setterType, IVisualElement element, IVisualLineage visualLineage)
        {
            return ThrowException<T>();
        }

        public T GetStyleSetter<T>(StyleSetterType setterType, VisualStateType type, IVisualElement element,
                                   IVisualLineage visualLineage)
        {
            return ThrowException<T>();
        }

        public void RegisterStyleSetter<T>(StyleSetterType setterType, Object value, IVisualElement scope) where T : IVisualElement
        {
            ThrowException<Object>();
        }

        public void CoerceIsChanged()
        {
            ThrowException<Object>();
        }

        public IStyleVariableAccessor VariableAccessor => ThrowException<IStyleVariableAccessor>();
    }
}

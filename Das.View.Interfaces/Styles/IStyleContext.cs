using System;
using System.Collections.Generic;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public interface IStyleContext : IStyleProvider
    {
        void RegisterStyle(IStyle style, IVisualElement scope);

        /// <summary>
        /// Registers a single style setter at the element level
        /// </summary>
        void RegisterStyleSetter(IVisualElement element, StyleSetters setter,
            Object value);

        /// <summary>
        /// Registers a single style setter at the type level.
        /// </summary>
        void RegisterStyleSetter<T>(StyleSetters setter,
            Object value, IVisualElement scope) where T : IVisualElement;

        IEnumerable<IStyle> GetStylesForElement(IVisualElement element);
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public interface IStyleContext : IStyleProvider
    {
        IEnumerable<IStyle> GetStylesForElement(IVisualElement element);

        /// <summary>
        /// registers a style on the application level
        /// </summary>
        void RegisterStyle(IStyle style);

        void RegisterStyle(IStyle style, 
                           IVisualElement scope);

        /// <summary>
        ///     Registers a single style setter at the element level
        /// </summary>
        void RegisterStyleSetter(IVisualElement element, 
                                 StyleSetter setter,
                                 Object value);

        /// <summary>
        ///     Registers a single style setter at the type level.
        /// </summary>
        void RegisterStyleSetter<T>(StyleSetter setter,
                                    Object value, IVisualElement scope) where T : IVisualElement;
    }
}
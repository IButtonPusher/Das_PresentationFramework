using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Styles
{
    public interface IStyleRegistry
    {
        /// <summary>
        ///     Registers a single style setter at the element level
        /// </summary>
        void RegisterStyleSetter(IVisualElement element,
                                 StyleSetterType setterType,
                                 Object value);

        void RegisterStyleSetter(IVisualElement element,
                                 StyleSetterType setterType,
                                 VisualStateType type,
                                 Object value);
    }
}

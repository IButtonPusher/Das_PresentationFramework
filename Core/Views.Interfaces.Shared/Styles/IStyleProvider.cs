using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;

namespace Das.Views.Styles
{
    public interface IStyleProvider
    {
        T GetStyleSetter<T>(StyleSetterType setterType,
                            IVisualElement element);

        /// <summary>
        ///     Falls back to StyleSelector.None if a different value is passed and no setter is found
        /// </summary>
        T GetStyleSetter<T>(StyleSetterType setterType,
                            StyleSelector selector,
                            IVisualElement element);

        /// <summary>
        ///     Registers a single style setter at the element level
        /// </summary>
        void RegisterStyleSetter(IVisualElement element,
                                 StyleSetterType setterType,
                                 Object value);

        void RegisterStyleSetter(IVisualElement element,
                                 StyleSetterType setterType,
                                 StyleSelector selector,
                                 Object value);

        IColorPalette ColorPalette { get; }

        //IColor GetCurrentAccentColor();
    }
}
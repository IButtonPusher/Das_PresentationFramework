using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public interface IStyleProvider
    {
        T GetStyleSetter<T>(StyleSetter setter,
                            IVisualElement element);

        /// <summary>
        ///     Falls back to StyleSelector.None if a different value is passed and no setter is found
        /// </summary>
        T GetStyleSetter<T>(StyleSetter setter,
                            StyleSelector selector,
                            IVisualElement element);

        /// <summary>
        ///     Registers a single style setter at the element level
        /// </summary>
        void RegisterStyleSetter(IVisualElement element,
                                 StyleSetter setter,
                                 Object value);

        void RegisterStyleSetter(IVisualElement element,
                                 StyleSetter setter,
                                 StyleSelector selector,
                                 Object value);

        IColor GetCurrentAccentColor();
    }
}
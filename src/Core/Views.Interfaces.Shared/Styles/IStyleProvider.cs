using System;
using System.Threading.Tasks;
using Das.Views.Colors;

namespace Das.Views.Styles
{
    public interface IStyleProvider : IThemeProvider
    {
        //T GetStyleSetter<T>(StyleSetterType setterType,
        //                    IVisualElement element);

        /// <summary>
        ///     Falls back to StyleSelector.None if a different value is passed and no setter is found
        /// </summary>
        T GetStyleSetter<T>(StyleSetterType setterType,
                            VisualStateType type,
                            IVisualElement element);

    }
}
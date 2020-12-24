using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Views.Colors;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public interface IStyleContext : IStyleRegistry,
                                     IChangeTracking,
                                     IThemeProvider
    {
        IEnumerable<IStyle> GetStylesForElement(IVisualElement element);

        /// <summary>
        ///     registers a style on the application level
        /// </summary>
        void RegisterStyle(IStyle style);

        void RegisterStyle(IStyle style,
                           IVisualElement scope);

        T GetStyleSetter<T>(StyleSetterType setterType,
                            IVisualElement element,
                            IVisualLineage visualLineage);

        /// <summary>
        ///     Falls back to StyleSelector.None if a different value is passed and no setter is found
        /// </summary>
        T GetStyleSetter<T>(StyleSetterType setterType,
                            VisualStateType type,
                            IVisualElement element,
                            IVisualLineage visualLineage);

        //void SetCurrentAccentColor(IColor color);

        /// <summary>
        ///     Registers a single style setter at the type level.
        /// </summary>
        void RegisterStyleSetter<T>(StyleSetterType setterType,
                                    Object value,
                                    IVisualElement scope)
            where T : IVisualElement;

        void CoerceIsChanged();

        IStyleVariableAccessor VariableAccessor { get; }
    }
}
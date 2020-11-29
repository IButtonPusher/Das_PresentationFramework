using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public interface IStyleContext : IStyleProvider,
                                     IChangeTracking
    {
        IEnumerable<IStyle> GetStylesForElement(IVisualElement element);

        /// <summary>
        ///     registers a style on the application level
        /// </summary>
        void RegisterStyle(IStyle style);

        void RegisterStyle(IStyle style,
                           IVisualElement scope);

        void SetCurrentAccentColor(IColor color);

        /// <summary>
        ///     Registers a single style setter at the type level.
        /// </summary>
        void RegisterStyleSetter<T>(StyleSetter setter,
                                    Object value, 
                                    IVisualElement scope) 
            where T : IVisualElement;

        void CoerceIsChanged();

        void PushVisual(IVisualElement visual);

        IVisualElement PopVisual();
    }
}
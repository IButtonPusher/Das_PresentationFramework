using System;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Construction.Styles;
using Das.Views.Core.Drawing;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Gdi.Shared.Static
{
    public readonly struct StaticViewState : IViewState
    {
        public StaticViewState(Double zoomLevel,
                               IColorPalette colorPalette)
        {
            _defaultStyle = DefaultStyle.Instance;
            ZoomLevel = zoomLevel;
            ColorPalette = colorPalette; //new DefaultColorPalette();
            StyleContext = new BaseStyleContext(DefaultStyle.Instance, colorPalette,
                new StyleVariableAccessor()); //DefaultStyleContext.Instance;
        }

        public T GetStyleSetter<T>(StyleSetterType setterType,
                                   IVisualElement element)
        {
            if (_defaultStyle[setterType] is T good) return good;

            return default!;
        }

        public T GetStyleSetter<T>(StyleSetterType setterType,
                                   VisualStateType type,
                                   IVisualElement element)
        {
            return GetStyleSetter<T>(setterType, element);
        }

        public void RegisterStyleSetter(IVisualElement element,
                                        StyleSetterType setterType,
                                        Object value)
        {
            throw new NotSupportedException();
        }

        public void RegisterStyleSetter(IVisualElement element,
                                        StyleSetterType setterType,
                                        VisualStateType type,
                                        Object value)
        {
            throw new NotSupportedException();
        }

        public IColorPalette ColorPalette { get; }


        public Double ZoomLevel { get; }

        private readonly DefaultStyle _defaultStyle;

        public IStyleContext StyleContext { get; }
    }
}
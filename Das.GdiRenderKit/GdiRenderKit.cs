using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Views;
using Das.Views.Controls;
using Das.Views.Core;
using Das.Views.Gdi;
using Das.Views.Gdi.Controls;
using Das.Views.Input;
using Das.Views.Rendering;

namespace Das.Gdi.Kits
{
    public class GdiRenderKit : BaseRenderKit,
                                IRenderKit,
                                IVisualSurrogateProvider
    {
        public GdiRenderKit(IViewPerspective viewPerspective,
                            IWindowProvider<IVisualHost> windowProvider)
        {
            _surrogateInstances = new Dictionary<IVisualElement, IVisualSurrogate>();
            _surrogateTypeBuilders = new Dictionary<Type, Func<IVisualElement,IVisualSurrogate>>();

            _surrogateTypeBuilders[typeof(HtmlPanel)] = GetHtmlPanelSurrogate;

            MeasureContext = new GdiMeasureContext(this);
            RenderContext = new GdiRenderContext(MeasureContext,
                viewPerspective, MeasureContext.Graphics, this);

            _containedObjects[typeof(IImageProvider)] = RenderContext;
            _containedObjects[typeof(IUiProvider)] = new GdiUiProvider();

            windowProvider.WindowShown += OnWindowShown;
        }

        private IVisualSurrogate GetHtmlPanelSurrogate(IVisualElement element)
        {
            if (!(_window is {} window) || !(_windowControl is {} control))
                throw new InvalidOperationException();

            return window.Invoke(() =>
            {
                var v = new HtmlViewSurrogate(element, control);
                control.Controls.Add(v);
                return v;
            });
        }

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;

        IRenderContext IRenderKit.RenderContext => RenderContext;

        public void EnsureSurrogate(ref IVisualElement element)
        {
            if (_surrogateInstances.TryGetValue(element, out var surrogate))
                element = surrogate;
            else if (_surrogateTypeBuilders.TryGetValue(element.GetType(), out var bldr))
            {
                var res = bldr(element);
                _surrogateInstances[element] = res;
                element = res;
            }
        }

        public GdiMeasureContext MeasureContext { get; }

        public GdiRenderContext RenderContext { get; }

        private void OnWindowShown(IVisualHost window)
        {
            _window = window;

            if (window is Control ctrl && ctrl.Controls.Count == 1)
                _windowControl = ctrl.Controls[0];
            else throw new InvalidOperationException();

            _inputContext = new InputContext(window, new BaseInputHandler(RenderContext));
        }

        private readonly Dictionary<IVisualElement, IVisualSurrogate> _surrogateInstances;
        private readonly Dictionary<Type, Func<IVisualElement, IVisualSurrogate>> _surrogateTypeBuilders;

        // ReSharper disable once NotAccessedField.Local
        private IInputContext? _inputContext;
        private IVisualHost? _window;
        private Control? _windowControl;
    }
}
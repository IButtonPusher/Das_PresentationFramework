using System;
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
                                       IRenderKit
    {
        public GdiRenderKit(IViewPerspective viewPerspective,
                            IWindowProvider<IVisualHost> windowProvider)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            RegisterSurrogate<HtmlPanel>(GetHtmlPanelSurrogate);

            MeasureContext = new GdiMeasureContext(this);
            RenderContext = new GdiRenderContext(viewPerspective, MeasureContext.Graphics, this);

            Resolver.ResolveTo<IImageProvider, GdiRenderContext>(RenderContext);
            Resolver.ResolveTo<IUiProvider, GdiUiProvider>(new GdiUiProvider());

            //_containedObjects[typeof(IImageProvider)] = RenderContext;
            //_containedObjects[typeof(IUiProvider)] = new GdiUiProvider();

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

      

        // ReSharper disable once NotAccessedField.Local
        private IInputContext? _inputContext;
        private IVisualHost? _window;
        private Control? _windowControl;
    }
}
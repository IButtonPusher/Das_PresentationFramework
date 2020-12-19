using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Das.Container;
using Das.Extensions;
using Das.Gdi.Controls;
using Das.Gdi.Core;
using Das.Gdi.Kits;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Mvvm;
using Das.Views.Styles;

namespace Das.Gdi
{
    public class GdiProvider : IWindowProvider<ViewWindow>, 
                               IBootStrapper
    {
        public GdiRenderKit RenderKit { get; }

        // ReSharper disable once UnusedMember.Global
        public GdiProvider(GdiRenderKit renderKit)
        {
            RenderKit = renderKit;
            VisualBootstrapper = renderKit.VisualBootstrapper;
        }

        public GdiProvider(IResolver container)
        {
            RenderKit = GetKit(this, container);
            VisualBootstrapper = RenderKit.VisualBootstrapper;
        }

        // ReSharper disable once UnusedMember.Global
        public GdiProvider() : this(new BaseResolver())
        {
            
        }

        private static GdiRenderKit GetKit(IWindowProvider<IVisualHost> windowProvider,
                                           IResolver container)
        {
            var perspective = new BasePerspective();
            var kit = new GdiRenderKit(perspective, windowProvider,
                new BaseStyleContext(DefaultStyle.Instance, new DefaultColorPalette()),
                container);
            return kit;
        }

        public ViewWindow Show<TViewModel>(TViewModel viewModel, 
                                           IBindableElement view)
            //where TViewModel : IViewModel
        {
            var styleContext = RenderKit.StyleContext;

            var control = new GdiHostedElement(view, styleContext);
            var form = new ViewWindow(control);
            Cook(form);

            //var myScreen = Screen.FromControl(form);
            //var area = myScreen.WorkingArea;
            //var size = new ValueSize(area.Width, area.Height);

            //var iWant = renderer.GetContentSize(size);

            //var hwnd = new WindowInteropHelper( this ).EnsureHandle();
            //var monitor = NativeMethods.MonitorFromWindow( hwnd, NativeMethods.MONITOR_DEFAULTTONEAREST );

            view.DataContext = viewModel;
            //view.SetDataContext(viewModel);

            WindowShown?.Invoke(form);

            return form;
        }

        //public ViewWindow Show<TViewModel>(TViewModel viewModel, 
        //                                   IView view) 
        //    //where TViewModel : IViewModel
        //{
        //    var styleContext = view.StyleContext;

        //    var control = new GdiHostedElement(view, styleContext);
        //    var form = new ViewWindow(control);
        //    Cook(form);

        //    //System.Windows.SystemParameters.PrimaryScreenWidth System.Windows.SystemParameters.PrimaryScreenHeight

        //    //renderer.GetContentSize()

        //   // view.DataContext = viewModel;
        //    //view.SetDataContext(viewModel);

        //    WindowShown?.Invoke(form);

        //    return form;
        //}


        public ViewWindow Show<TRectangle>(IView view,
                                           TRectangle rect)
            where TRectangle : IRectangle
        {
            var styleContext = view.StyleContext;

            var control = new GdiHostedElement(view, styleContext);
            var form = new ViewWindow(control);
            Cook(form);

            form.Bounds = GdiTypeConverter.GetRect(rect);

            WindowShown?.Invoke(form);

            return form;
        }

        public ViewWindow Show(IView view)
        {
            var styleContext = view.StyleContext;

            var control = new GdiHostedElement(view, styleContext);
            var form = new ViewWindow(control);
            Cook(form);

            var viewWidth = view.Width ?? 0;
            var viewHeight = view.Height ?? 0;

            if (viewWidth.IsNotZero() && viewHeight.IsNotZero())
                form.Size = GdiTypeConverter.GetSize(viewWidth, viewHeight);
            
            WindowShown?.Invoke(form);

            return form;

            //return Show(view.DataContext, view);
        }

        //public VisualForm Show(IVisualRenderer visual)
        //{
        //    var control = new HostedVisualControl(visual);
        //    var form = new VisualForm(visual, control);
        //    Cook(form);

        //    return form;
        //}

        public event Action<ViewWindow>? WindowShown;

        public void Run(IView view)
        {
            var window = Show(view);
            
            Application.Run(window);
        }
        
        //public void Run<TViewModel>(TViewModel viewModel, 
        //                            IView view)
        //{
        //    var window = Show(viewModel, view);
            
        //    Application.Run(window);
        //}

        public IVisualBootstrapper VisualBootstrapper { get; }

        // ReSharper disable once UnusedMember.Global
        public GdiHostedElement Host<TViewModel>(TViewModel viewModel, 
                                                 IView view)
            where TViewModel : IViewModel
        {
            var styleContext = view.StyleContext;
            var control = new GdiHostedElement(view, styleContext);
            //control.DataContext = viewModel;
            Cook(control);

            view.DataContext = viewModel;
            //view.SetDataContext(viewModel);

            return control;
        }

        // ReSharper disable once UnusedMember.Global
        public GdiHostedElement HostStatic<TViewModel>(TViewModel viewModel
                                                       , IView view)
        {
            var styleContext = view.StyleContext;
            var control = new GdiHostedElement(view, styleContext);
            var renderer = new BitmapRenderer(control,
                RenderKit.MeasureContext, RenderKit.RenderContext);

            view.DataContext = viewModel;
            //view.PropertyChanged += (o, e) =
            //view.SetDataContext(viewModel);

            control.BackingBitmap = renderer.DoRender();
            //control.DataContextChanged += (o, e) => { control.BackingBitmap = renderer.DoRender(); };

            return control;
        }

        //private void OnViewPropertyChanged(Object sender, 
        //                                   PropertyChangedEventArgs e)
        //{
        //    switch (e.PropertyName)
        //    {
        //        nameof(IBindableElement.DataContext)
        //    }
        //}

        // ReSharper disable once UnusedMethodReturnValue.Local
        private IRenderer<Bitmap> Cook(IViewHost<Bitmap> form)
        {
            var renderer = new BitmapRenderer(form,
                RenderKit.MeasureContext, RenderKit.RenderContext);
            var _ = new LoopViewUpdater<Bitmap>(form, renderer);
            return renderer;

        }

       
    }
}
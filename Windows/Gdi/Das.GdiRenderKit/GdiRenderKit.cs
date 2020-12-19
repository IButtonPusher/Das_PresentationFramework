﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Container;
using Das.Serializer;
using Das.Views;
using Das.Views.Controls;
using Das.Views.Core;
using Das.Views.Core.Geometry;
using Das.Views.Gdi;
using Das.Views.Gdi.Controls;
using Das.Views.Input;
using Das.Views.Rendering;
using Das.Views.Styles;
using Gdi.Shared;

namespace Das.Gdi.Kits
{
    public class GdiRenderKit : BaseRenderKit,
                                       IRenderKit,
                                       IDisplayMetrics
    {
        public IStyleContext StyleContext { get; }

        private readonly IWindowProvider<IVisualHost> _windowProvider;

        public GdiRenderKit(IViewPerspective viewPerspective,
                            IWindowProvider<IVisualHost> windowProvider,
                            IStyleContext styleContext,
                            IResolver container)
        : base(container, styleContext, Serializer.AttributeParser, Serializer.TypeInferrer,
            Serializer.TypeManipulator)
        {
            StyleContext = styleContext;
            _windowProvider = windowProvider;
            // ReSharper disable once VirtualMemberCallInConstructor
            RegisterSurrogate<HtmlPanel>(GetHtmlPanelSurrogate);

            _imageProvider = new GdiImageProvider();
            var lastMeasure = new Dictionary<IVisualElement, ValueSize>();

            MeasureContext = new GdiMeasureContext(this, lastMeasure, styleContext);
            RenderContext = new GdiRenderContext(viewPerspective, 
                MeasureContext.Graphics, this, lastMeasure,
                new Dictionary<IVisualElement, ValueCube>(), styleContext);

            Container.ResolveTo<IImageProvider>(_imageProvider);
            Container.ResolveTo<IUiProvider>(new GdiUiProvider(windowProvider));
            Container.ResolveTo(styleContext);

            windowProvider.WindowShown += OnWindowShown;
        }

        public GdiRenderKit(IViewPerspective viewPerspective,
                            IWindowProvider<IVisualHost> windowProvider,
                            IStyleContext styleContext)
        : this(viewPerspective, windowProvider, styleContext, new BaseResolver())
        {
           
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
            _windowProvider.WindowShown -= OnWindowShown;

            _window = window;
            _imageProvider.SetVisualHost(window);

            if (window is Control ctrl && ctrl.Controls.Count == 1)
                _windowControl = ctrl.Controls[0];
            else throw new InvalidOperationException();

            _inputContext = new InputContext(window, new BaseInputHandler(RenderContext), ctrl.Handle);
        }

      

        // ReSharper disable once NotAccessedField.Local
        private IInputContext? _inputContext;
        private IVisualHost? _window;
        private Control? _windowControl;
        private readonly GdiImageProvider _imageProvider;

        protected static readonly DasSerializer Serializer = new DasSerializer();

        public Double ZoomLevel => RenderContext.ViewState?.ZoomLevel ?? 1;
    }
}
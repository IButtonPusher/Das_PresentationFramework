using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Container;
using Das.Serializer;
using Das.Views;
using Das.Views.Colors;
using Das.Views.Construction;
using Das.Views.Controls;
using Das.Views.Core;
using Das.Views.Core.Geometry;
using Das.Views.Gdi;
using Das.Views.Gdi.Controls;
using Das.Views.Images.Svg;
using Das.Views.Input;
using Das.Views.Layout;
using Das.Views.Rendering;
using Das.Views.Styles;
using Gdi.Shared;
// ReSharper disable UnusedMember.Global

namespace Das.Gdi.Kits
{
    public class GdiRenderKit : BaseRenderKit,
                                IRenderKit,
                                IDisplayMetrics
    {
        public GdiRenderKit(IViewPerspective viewPerspective,
                            IWindowProvider<IVisualHost> windowProvider,
                            IResolver resolver,
                            IThemeProvider styleContext,
                            IVisualBootstrapper visualBootstrapper,
                            IViewInflater viewInflater,
                            IImageProvider imageProvider)
            : base(resolver, visualBootstrapper, viewInflater,
                new Dictionary<IVisualElement, ValueCube>(), imageProvider)
        {
            _windowProvider = windowProvider;
            Init(windowProvider, styleContext, viewPerspective, _renderPositions,
                ref _imageProvider!, ref _measureContext!, ref _renderContext!);
        }

        /// <summary>
        /// Minimal constructor
        /// </summary>
        public GdiRenderKit(IViewPerspective viewPerspective,
                            IWindowProvider<IVisualHost> windowProvider)
            : base(GetDefaultImageBuilder(), Serializer,
                new SvgPathBuilder(GetDefaultImageBuilder(), Serializer), null)
        {
            _windowProvider = windowProvider;
            Init(windowProvider, BaselineThemeProvider.Instance, viewPerspective, _renderPositions,
                ref _imageProvider!, ref _measureContext!, ref _renderContext!);
        }

        public GdiRenderKit(IViewPerspective viewPerspective,
                            IWindowProvider<IVisualHost> windowProvider,
                            IResolver container)
            : base(GetDefaultImageBuilder(), Serializer,
                new SvgPathBuilder(GetDefaultImageBuilder(), Serializer), container)
        {
            _windowProvider = windowProvider;
            Init(windowProvider, BaselineThemeProvider.Instance, viewPerspective, _renderPositions,
                ref _imageProvider!, ref _measureContext!, ref _renderContext!);
        }

        private static GdiImageProvider GetDefaultImageBuilder() => _defaultImageBuilder ??= new GdiImageProvider();
        private static GdiImageProvider? _defaultImageBuilder;

        //public GdiRenderKit(IViewPerspective viewPerspective,
        //                    IWindowProvider<IVisualHost> windowProvider,
        //                    IThemeProvider themeProvider,
        //                    IResolver container)
        //    : base(container, themeProvider, Serializer.AttributeParser, Serializer.TypeInferrer,
        //        Serializer.TypeManipulator, new Dictionary<IVisualElement, ValueCube>(),
        //        new GdiImageProvider(), Serializer.AssemblyList, Serializer)
        //{
        //    _windowProvider = windowProvider;
        //    Init(windowProvider, themeProvider, viewPerspective, _renderPositions,
        //        ref _imageProvider!, ref _measureContext!, ref _renderContext!);
        //}

        //public GdiRenderKit(IViewPerspective viewPerspective,
        //                    IWindowProvider<IVisualHost> windowProvider,
        //                    IStyleContext styleContext)
        //    : this(viewPerspective, windowProvider, styleContext, new BaseResolver())
        //{
        //}

        //public GdiRenderKit(IWindowProvider<IVisualHost> windowProvider,
        //                    IViewPerspective viewPerspective,
        //                    IStyleContext styleContext,
        //                    IStringPrimitiveScanner attributeScanner,
        //                    ITypeInferrer typeInferrer,
        //                    IPropertyProvider propertyProvider,
        //                    IAssemblyList assemblyList,
        //                    IXmlSerializer xmlSerializer)
        //    : base(styleContext, attributeScanner, typeInferrer, propertyProvider,
        //        new Dictionary<IVisualElement, ValueCube>(),new GdiImageProvider(),
        //        assemblyList, xmlSerializer)
        //{
        //    _windowProvider = windowProvider;
        //    Init(windowProvider, styleContext, viewPerspective, 
        //            _renderPositions,
        //        ref _imageProvider!,
        //        ref _measureContext!, ref _renderContext!);
        //}

        //public GdiRenderKit(IWindowProvider<IVisualHost> windowProvider,
        //                    IViewPerspective viewPerspective,
        //                    IResolver resolver,
        //                    IStyleContext styleContext,
        //                    IStringPrimitiveScanner attributeScanner,
        //                    ITypeInferrer typeInferrer,
        //                    IPropertyProvider propertyProvider,
        //                    IVisualBootstrapper visualBootstrapper,
        //                    IVisualStyleProvider styleProvider,
        //                    IImageProvider imageProvider,
        //                    IAssemblyList assemblyList,
        //                    IXmlSerializer xmlSerializer)
        //    : base(resolver, attributeScanner, typeInferrer,
        //        propertyProvider, visualBootstrapper,
        //        new Dictionary<IVisualElement, ValueCube>(), styleProvider, imageProvider,
        //        assemblyList, xmlSerializer)
        //{
        //    _windowProvider = windowProvider;
        //    Init(windowProvider, styleContext, viewPerspective, _renderPositions,
        //        ref _imageProvider!,
        //        ref _measureContext!, ref _renderContext!);
        //}

        public Double ZoomLevel => RenderContext.ViewState?.ZoomLevel ?? 1;

        public Single Density => 1.0f; //todo:

        IMeasureContext IRenderKit.MeasureContext => _measureContext;

        IRenderContext IRenderKit.RenderContext => _renderContext;

        


        public GdiMeasureContext MeasureContext => _measureContext;

        public GdiRenderContext RenderContext => _renderContext;


        private IVisualSurrogate GetHtmlPanelSurrogate(IVisualElement element)
        {
            if (!(_window is { } window) || !(_windowControl is { } control))
                throw new InvalidOperationException();

            return window.Invoke(() =>
            {
                var v = new HtmlViewSurrogate(element, control);
                control.Controls.Add(v);
                return v;
            });
        }


        [SuppressMessage("ReSharper", "RedundantAssignment")]
        private void Init(IWindowProvider<IVisualHost> windowProvider,
                          IThemeProvider themeProvider,
                          IViewPerspective viewPerspective,
                          Dictionary<IVisualElement, ValueCube> renderPositions,
                          ref GdiImageProvider? imageProvider,
                          ref GdiMeasureContext measureContext,
                          ref GdiRenderContext renderContext)
        {
            imageProvider = new GdiImageProvider();
            var lastMeasure = new Dictionary<IVisualElement, ValueSize>();
            var visualLineage = new VisualLineage();

            measureContext = new GdiMeasureContext(this, lastMeasure,
                themeProvider, visualLineage, VisualBootstrapper.LayoutQueue);

            renderContext = new GdiRenderContext(viewPerspective,
                MeasureContext.Graphics, this, lastMeasure,
                renderPositions, themeProvider, visualLineage, VisualBootstrapper.LayoutQueue);

            Container.ResolveTo<IImageProvider>(imageProvider);
            Container.ResolveTo<IUiProvider>(new GdiUiProvider(windowProvider));
            Container.ResolveTo(themeProvider);

            windowProvider.WindowShown += OnWindowShown;

            RegisterSurrogate<HtmlPanel>(GetHtmlPanelSurrogate);
        }


        private void OnWindowShown(IVisualHost window)
        {
            _windowProvider.WindowShown -= OnWindowShown;

            _window = window;
            _imageProvider.SetVisualHost(window);

            if (window is Control ctrl && ctrl.Controls.Count == 1)
            {
                _windowControl = ctrl.Controls[0];
                Container.ResolveTo(_windowControl);
            }
            else throw new InvalidOperationException();

            _inputContext = new InputContext(window, new BaseInputHandler(RenderContext), _windowControl, 
                ctrl.Handle);
        }

        protected static readonly DasSerializer Serializer = new();
        private readonly GdiImageProvider _imageProvider;
        private readonly GdiMeasureContext _measureContext;

        private readonly GdiRenderContext _renderContext;
        private readonly IWindowProvider<IVisualHost> _windowProvider;


        // ReSharper disable once NotAccessedField.Local
        private IInputContext? _inputContext;
        private IVisualHost? _window;
        private Control? _windowControl;
    }
}
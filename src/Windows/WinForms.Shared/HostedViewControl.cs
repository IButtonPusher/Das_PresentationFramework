using Das.Views.Panels;
using System;
using Das.Views;
using Das.Views.Colors;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Hosting;
using Das.Views.Rendering;
using Das.Views.Windows;
using Das.Views.Winforms;


namespace WinForms.Shared
{
    public abstract class HostedViewControl : HostedControl,
                                              IBoundElementContainer, 
                                              IWindowsViewHost
    {
        private readonly IThemeProvider _themeProvider;

        protected HostedViewControl(IVisualElement view,
                                    IThemeProvider themeProvider)
                                    //IStyleContext styleContext)
            : this()//styleContext)
        {
            _themeProvider = themeProvider;
            View = view;
        }

#pragma warning disable 8618
        protected HostedViewControl()//IStyleContext styleContext)
#pragma warning restore 8618
        {
            //StyleContext = styleContext;
            _zoomLevel = 1;
        }

        public IVisualElement View { get; protected set; }

        public IVisualElement Element { get; set; }
       

        public void SetView(IVisualElement view) => View = view;

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public SizeToContent SizeToContent { get; set; }

        //public IStyleContext StyleContext { get; set; }

        public Thickness RenderMargin { get; } = Thickness.Empty;

        private Double _zoomLevel;

        public Double ZoomLevel
        {
            get => _zoomLevel;
            set
            {
                _zoomLevel = value;
                Element.InvalidateMeasure();
            }
        }

        public Single Density => 1.0f; //todo:

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            View.Width = Width;
            View.Height = Height;
        }

        public void AcceptChanges()
        {
            View.AcceptChanges(ChangeType.Measure);
            View.AcceptChanges(ChangeType.Arrange);
        }

        public virtual Boolean IsChanged => View.IsRequiresMeasure || View.IsRequiresArrange; //_isChanged;

        public IPoint2D GetOffset(IPoint2D input)
        {
            return Point2D.Empty;
        }

        //public IVisualRenderer Visual => View;
        public IColorPalette ColorPalette => _themeProvider.ColorPalette;
    }
}

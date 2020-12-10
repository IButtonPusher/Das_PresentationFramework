using Das.Views.Panels;
using Das.Views.Styles;
using System;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Hosting;
using Das.Views.Rendering;
using Das.Views.Windows;
using Das.Views.Winforms;
using Das.Views.Mvvm;


namespace WinForms.Shared
{
    public abstract class HostedViewControl : HostedControl, 
        IBoundElementContainer, IWindowsViewHost
    {
        protected HostedViewControl(IView view, 
                                    IStyleContext styleContext)
            : this(styleContext)
        {
            View = view;
        }

#pragma warning disable 8618
        protected HostedViewControl(IStyleContext styleContext)
#pragma warning restore 8618
        {
            StyleContext = styleContext;
            _zoomLevel = 1;
        }
     

        public event EventHandler? DataContextChanged;

        //private Boolean _isChanged;
        private IViewModel? _dataContext;

        public IView View { get; protected set; }

        public IVisualElement Element { get; set; }
        public IViewModel? DataContext
        {
            get => _dataContext;
            set
            {
                _dataContext = value;
                View.SetDataContext(value);
                //_isChanged = true;
                DataContextChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetView(IView view) => View = view;

        public SizeToContent SizeToContent { get; set; }

        public IStyleContext StyleContext { get; set; }

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

        public T GetStyleSetter<T>(StyleSetter setter, IVisualElement element)
            => StyleContext.GetStyleSetter<T>(setter, element);

        public T GetStyleSetter<T>(StyleSetter setter, StyleSelector selector, IVisualElement element)
        {
            return StyleContext.GetStyleSetter<T>(setter, selector, element);
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetter setter, 
                                        Object value)
        {
            StyleContext.RegisterStyleSetter(element, setter, value);
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetter setter, 
                                        StyleSelector selector, 
                                        Object value)
        {
            StyleContext.RegisterStyleSetter(element, setter, selector, value);
        }

        public IColorPalette ColorPalette => StyleContext.ColorPalette;

        public IColor GetCurrentAccentColor()
        {
            return StyleContext.GetCurrentAccentColor();
        }

        public void AcceptChanges()
        {
            View.AcceptChanges();
        }

        public virtual Boolean IsChanged => View.IsChanged;  //_isChanged;

        public IPoint2D GetOffset(IPoint2D input)
        {
            return Point2D.Empty;
        }

        //public IVisualRenderer Visual => View;
    }
}

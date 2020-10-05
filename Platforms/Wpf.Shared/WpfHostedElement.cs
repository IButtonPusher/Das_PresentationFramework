using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;
using Das.ViewModels;
using Size = System.Windows.Size;

namespace Das.Views.Wpf
{
    public class WpfHostedElement : ContentControl, IViewHost
    {
        public WpfHostedElement(IView view, 
                                IStyleContext styleContext,
                                IRenderContext renderContext)
        {
            View = view;
            StyleContext = styleContext;

            Loaded += OnLoaded;
            AvailableSize = new Core.Geometry.Size(1, 1);

            
        }



        protected override Size MeasureOverride(Size constraint)
        {
            AvailableSize.Width = constraint.Width;
            AvailableSize.Height = constraint.Height;

            return base.MeasureOverride(constraint);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            //var img = new WriteableBitmap(640, 480, 5, 5, new PixelFormat(), new BitmapPalette(
            //    new List<Color>()));

            //img.BackBuffer

            //drawingContext.DrawImage()
        }

        private void OnLoaded(Object sender, RoutedEventArgs e)
        {
            IsLoaded = true;
        }

        public Boolean IsLoaded { get; private set; }

        public Core.Geometry.Size AvailableSize { get; }

        public event Func<Task>? HostCreated;

        public event Action<ISize>? AvailableSizeChanged;

        public void Invoke(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        public async Task InvokeAsync(Action action)
        {
            await Application.Current.Dispatcher.InvokeAsync(action);
        }

        public T GetStyleSetter<T>(StyleSetters setter, IVisualElement element)
            => StyleContext.GetStyleSetter<T>(setter, element);

        public IView View { get; }

        public IStyleContext StyleContext { get; }

        public Double ZoomLevel
        {
            get => _zoomLevel;
            set
            {
                _zoomLevel = value;
                _isChanged = true;
            }
        }

        private Double _zoomLevel;
        private Boolean _isChanged;

        public IViewModel? DataContext { get; set; }

        public void Invalidate()
        {
            InvalidateMeasure();
            //TODO_IMPLEMENT_ME();
        }

        public Core.Geometry.Thickness RenderMargin { get; }= Core.Geometry.Thickness.Empty;

        public void AcceptChanges()
        {
            
        }

        public Boolean IsChanged => _isChanged;

        public IPoint2D GetOffset(IPoint2D input)
        {
            return Core.Geometry.Point2D.Empty;
        }

        public IVisualRenderer Visual => View;
    }
}

using System;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Colors;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Hosting;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Windows;
using Das.Views.Winforms;

namespace WinForms.Shared
{
   public abstract class HostedViewControl : HostedControl,
                                             IBoundElementContainer,
                                             IWindowsViewHost
   {
      protected HostedViewControl(IVisualElement view,
                                  IThemeProvider themeProvider,
                                  IVisualBootstrapper visualBootstrapper)
         : this()
      {
         _themeProvider = themeProvider;
         _visualBootstrapper = visualBootstrapper;
         _layoutQueue = visualBootstrapper.LayoutQueue;
         View = view;
      }

#pragma warning disable 8618
      protected HostedViewControl() 
#pragma warning restore 8618
      {
         //StyleContext = styleContext;
         _zoomLevel = 1;
      }

      public IVisualElement Element { get; set; }

      public IVisualElement View { get; protected set; }

      // ReSharper disable once UnusedAutoPropertyAccessor.Global
      public SizeToContent SizeToContent { get; set; }

      public Thickness RenderMargin { get; } = Thickness.Empty;

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

      public void AcceptChanges()
      {
         View.AcceptChanges(ChangeType.Measure);
         View.AcceptChanges(ChangeType.Arrange);
      }

      public virtual Boolean IsChanged
      {
         get
         {
            return _layoutQueue.HasVisualsNeedingLayout;
         }
      }

      public IPoint2D GetOffset(IPoint2D input)
      {
         return Point2D.Empty;
      }

      public IColorPalette ColorPalette => _themeProvider.ColorPalette;


      public void SetView(IVisualElement view)
      {
         View = view;
      }

      protected override void OnSizeChanged(EventArgs e)
      {
         base.OnSizeChanged(e);

         View.Width = Width;
         View.Height = Height;
      }

      protected readonly ILayoutQueue _layoutQueue;
      private readonly IThemeProvider _themeProvider;
      protected readonly IVisualBootstrapper _visualBootstrapper;

      private Double _zoomLevel;
   }
}

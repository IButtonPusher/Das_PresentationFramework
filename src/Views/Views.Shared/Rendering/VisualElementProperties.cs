using System;
using System.Threading.Tasks;
using Das.Views.BoxModel;
using Das.Views.Controls;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.DependencyProperties;
using Das.Views.Transforms;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;

#endif
namespace Das.Views
{
   public partial class VisualElement
   {
      public virtual Boolean IsRequiresMeasure
      {
         get => _isRequiresMeasure;
         protected set =>
            //if (value && Interlocked.Add(ref _trued, 1) % 50 == 0)
            //   Debug.WriteLine("required measure: " + _trued + " times " + this);
            SetValue(ref _isRequiresMeasure, value);
      }

      //private Int32 _trued;

      public virtual Boolean IsRequiresArrange
      {
         get => _isRequiresArrange;
         protected set => SetValue(ref _isRequiresArrange, value);
      }


      public QuantifiedDouble? Width
      {
         get => WidthProperty.GetValue(_me);
         set => WidthProperty.SetValue(this, value);
      }

      public QuantifiedDouble? Height
      {
         get => HeightProperty.GetValue(_me);
         set => HeightProperty.SetValue(this, value);
      }

      public HorizontalAlignments HorizontalAlignment
      {
         get => HorizontalAlignmentProperty.GetValue(_me);
         set => HorizontalAlignmentProperty.SetValue(this, value);
      }

      public VerticalAlignments VerticalAlignment
      {
         get => VerticalAlignmentProperty.GetValue(_me);
         set => VerticalAlignmentProperty.SetValue(this, value);
      }

      public IBrush? Background
      {
         get => BackgroundProperty.GetValue(_me);
         set => BackgroundProperty.SetValue(this, value);
      }

      public QuantifiedThickness Margin
      {
         get => MarginProperty.GetValue(_me);
         set => MarginProperty.SetValue(this, value);
      }

      public virtual Int32 Id { get; }

      public static readonly DependencyProperty<IVisualElement, String?> ClassProperty =
         DependencyProperty<IVisualElement, String?>.Register(
            nameof(Class), default);

      public String? Class
      {
         get => ClassProperty.GetValue(_me);
         set => ClassProperty.SetValue(this, value);
      }


      public virtual Boolean IsClipsContent { get; set; }

      private Action<IVisualElement>? _disposed;

      #if DEBUG
      private readonly System.Collections.Generic.HashSet<Action<IVisualElement>> _disposeCheck = new ();

      public event Action<IVisualElement>? Disposed
      {
         add
         {
            if (value is not { } val)
               return;
            if (!_disposeCheck.Add(val))
            {
            }

            _disposed += val;
         }
         remove => _disposed -= value;
      }

      #else
        public event Action<IVisualElement>? Disposed;

      #endif


      public static readonly DependencyProperty<IVisualElement, IVisualTemplate?> TemplateProperty =
         DependencyProperty<IVisualElement, IVisualTemplate?>.Register(
            nameof(Template), default);

      public IVisualTemplate? Template
      {
         get => TemplateProperty.GetValue(_me);
         set => TemplateProperty.SetValue(this, value);
      }

      public Double Opacity
      {
         get => OpacityProperty.GetValue(_me);
         set => OpacityProperty.SetValue(this, value);
      }

      public static readonly DependencyProperty<IVisualElement, Boolean> IsEnabledProperty =
         DependencyProperty<IVisualElement, Boolean>.Register(
            nameof(IsEnabled),
            default);

      public Boolean IsEnabled
      {
         get => IsEnabledProperty.GetValue(_me);
         set => IsEnabledProperty.SetValue(this, value);
      }

      public static readonly DependencyProperty<IVisualElement, ILabel?> BeforeLabelProperty =
         DependencyProperty<IVisualElement, ILabel?>.Register(
            nameof(BeforeLabel),
            default);

      public ILabel? BeforeLabel
      {
         get => BeforeLabelProperty.GetValue(_me);
         set => BeforeLabelProperty.SetValue(this, value);
      }

      public static readonly DependencyProperty<IVisualElement, ILabel?> AfterLabelProperty =
         DependencyProperty<IVisualElement, ILabel?>.Register(
            nameof(AfterLabel),
            default);

      public ILabel? AfterLabel
      {
         get => AfterLabelProperty.GetValue(_me);
         set => AfterLabelProperty.SetValue(this, value);
      }


      public static readonly DependencyProperty<IVisualElement, QuantifiedDouble?> WidthProperty =
         DependencyProperty<IVisualElement, QuantifiedDouble?>.Register(nameof(Width), null,
            PropertyMetadata.AffectsMeasure);

      public static readonly DependencyProperty<IVisualElement, QuantifiedDouble?> HeightProperty =
         DependencyProperty<IVisualElement, QuantifiedDouble?>.Register(nameof(Height), null);


      public static readonly DependencyProperty<IVisualElement, VerticalAlignments> VerticalAlignmentProperty =
         DependencyProperty<IVisualElement, VerticalAlignments>.Register(nameof(VerticalAlignment),
            VerticalAlignments.Default);

      public static readonly DependencyProperty<IVisualElement, HorizontalAlignments> HorizontalAlignmentProperty =
         DependencyProperty<IVisualElement, HorizontalAlignments>.Register(nameof(HorizontalAlignment),
            HorizontalAlignments.Default);

      public static readonly DependencyProperty<IVisualElement, IBrush?> BackgroundProperty =
         DependencyProperty<IVisualElement, IBrush?>.Register(nameof(Background), default);

      public static readonly DependencyProperty<IVisualElement, QuantifiedThickness> MarginProperty =
         DependencyProperty<IVisualElement, QuantifiedThickness>.Register(nameof(Margin),
            QuantifiedThickness.Empty, PropertyMetadata.AffectsMeasure);

      public static readonly DependencyProperty<IVisualElement, Double> OpacityProperty =
         DependencyProperty<IVisualElement, Double>.Register(nameof(Opacity), 1.0);


      public static readonly DependencyProperty<IVisualElement, Visibility> VisibilityProperty =
         DependencyProperty<IVisualElement, Visibility>.Register(
            nameof(Visibility),
            default);

      public Visibility Visibility
      {
         get => VisibilityProperty.GetValue(_me);
         set => VisibilityProperty.SetValue(this, value);
      }

      public static readonly DependencyProperty<IVisualElement, QuantifiedThickness> BorderRadiusProperty =
         DependencyProperty<IVisualElement, QuantifiedThickness>.Register(
            nameof(BorderRadius), QuantifiedThickness.Empty);

      public QuantifiedThickness BorderRadius
      {
         get => BorderRadiusProperty.GetValue(_me);
         set => BorderRadiusProperty.SetValue(this, value);
      }

      public static readonly DependencyProperty<IVisualElement, VisualBorder> BorderProperty =
         DependencyProperty<IVisualElement, VisualBorder>.Register(
            nameof(Border), VisualBorder.Empty);

      public VisualBorder Border
      {
         get => BorderProperty.GetValue(_me);
         set => BorderProperty.SetValue(this, value);
      }


      public static readonly DependencyProperty<IVisualElement, Int32> ZIndexProperty =
         DependencyProperty<IVisualElement, Int32>.Register(
            nameof(ZIndex),
            default);

      public Int32 ZIndex
      {
         get => ZIndexProperty.GetValue(_me);
         set => ZIndexProperty.SetValue(this, value);
      }

      public static readonly DependencyProperty<IVisualElement, QuantifiedDouble?> LeftProperty =
         DependencyProperty<IVisualElement, QuantifiedDouble?>.Register(
            nameof(Left),
            default);

      public QuantifiedDouble? Left
      {
         get => LeftProperty.GetValue(_me);
         set => LeftProperty.SetValue(this, value);
      }

      public static readonly DependencyProperty<IVisualElement, QuantifiedDouble?> TopProperty =
         DependencyProperty<IVisualElement, QuantifiedDouble?>.Register(
            nameof(Top),
            default);

      public QuantifiedDouble? Top
      {
         get => TopProperty.GetValue(_me);
         set => TopProperty.SetValue(this, value);
      }


      public static readonly DependencyProperty<IVisualElement, QuantifiedDouble?> RightProperty =
         DependencyProperty<IVisualElement, QuantifiedDouble?>.Register(
            nameof(Right),
            default);

      public QuantifiedDouble? Right
      {
         get => RightProperty.GetValue(_me);
         set => RightProperty.SetValue(this, value);
      }

      public static readonly DependencyProperty<IVisualElement, QuantifiedDouble?> BottomProperty =
         DependencyProperty<IVisualElement, QuantifiedDouble?>.Register(
            nameof(Bottom),
            default);

      public QuantifiedDouble? Bottom
      {
         get => BottomProperty.GetValue(_me);
         set => BottomProperty.SetValue(this, value);
      }

      public static readonly DependencyProperty<IVisualElement, TransformationMatrix> TransformProperty =
         DependencyProperty<IVisualElement, TransformationMatrix>.Register(
            nameof(Transform), TransformationMatrix.Identity, PropertyMetadata.AffectsArrange);

      public TransformationMatrix Transform
      {
         get => TransformProperty.GetValue(_me);
         set => TransformProperty.SetValue(this, value);
      }

      public static readonly DependencyProperty<IVisualElement, IBoxShadow> BoxShadowProperty =
         DependencyProperty<IVisualElement, IBoxShadow>.Register(
            nameof(BoxShadow), BoxModel.BoxShadow.Empty);

      public IBoxShadow BoxShadow
      {
         get => BoxShadowProperty.GetValue(_me);
         set => BoxShadowProperty.SetValue(this, value);
      }

      public virtual Boolean IsDisposed => _isDisposed;

      private Boolean _isRequiresArrange;
      private Boolean _isRequiresMeasure;
   }
}

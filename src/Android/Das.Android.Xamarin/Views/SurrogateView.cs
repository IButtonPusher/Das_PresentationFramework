using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Android.Content;
using Android.Views;
using Android.Widget;
using Das.Views;
using Das.Views.BoxModel;
using Das.Views.Controls;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Styles.Application;
using Das.Views.Styles.Declarations;
using Das.Views.Templates;
using Das.Views.Transforms;

namespace Das.Xamarin.Android
{
    public abstract class SurrogateView : FrameLayout, //ViewGroup,
                                          IVisualSurrogate
    {
        public SurrogateView(Context? context,
                             IVisualElement replacingVisual,
                             View nativeView,
                             ViewGroup viewGroup) : base(context)
        {
            _viewGroup = viewGroup;
            ReplacingVisual = replacingVisual;
            //replacingVisual.PropertyChanged += OnControlPropertyChanged;
            NativeView = nativeView;
            AddView(nativeView);
        }

        void IMeasureAndArrange.InvalidateMeasure()
        {
            ReplacingVisual.InvalidateMeasure();
        }

        void IMeasureAndArrange.InvalidateArrange()
        {
            ReplacingVisual.InvalidateArrange();
        }

        Boolean IMeasureAndArrange.IsRequiresMeasure => ReplacingVisual.IsRequiresMeasure;

        Boolean IMeasureAndArrange.IsRequiresArrange => ReplacingVisual.IsRequiresArrange;

        void IMeasureAndArrange.AcceptChanges(ChangeType changeType)
        {
            ReplacingVisual.AcceptChanges(changeType);
        }

        public abstract void Arrange<TRenderSize>(TRenderSize availableSpace,
                                                  IRenderContext renderContext) where TRenderSize : IRenderSize;

        //void IVisualRenderer.Arrange<TRenderSize>(TRenderSize availableSpace,
        //                                          IRenderContext renderContext)
        //{
        //    ReplacingVisual.Arrange(availableSpace, renderContext);
        //}

        public virtual ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
                                                      IMeasureContext measureContext)
            where TRenderSize : IRenderSize
        {
            return availableSpace.ToValueSize();
            //return ReplacingVisual.Measure(availableSpace, measureContext);
        }

        ValueRenderRectangle IVisualRenderer.ArrangedBounds
        {
            get => ReplacingVisual.ArrangedBounds;
            set => ReplacingVisual.ArrangedBounds = value;
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => ReplacingVisual.PropertyChanged += value;
            remove => ReplacingVisual.PropertyChanged -= value;
        }

        IVisualTemplate? ITemplatableVisual.Template
        {
            get => ReplacingVisual.Template;
            set => ReplacingVisual.Template = value;
        }

        Boolean IEquatable<IVisualElement>.Equals(IVisualElement other)
        {
            return ReplacingVisual.Equals(other);
        }

        QuantifiedDouble? IBoxValue<QuantifiedDouble?>.Left => ReplacingVisual.Left;

        QuantifiedDouble? IBoxValue<QuantifiedDouble?>.Right => ReplacingVisual.Right;

        QuantifiedDouble? IBoxValue<QuantifiedDouble?>.Top => ReplacingVisual.Top;

        QuantifiedDouble? IBoxValue<QuantifiedDouble?>.Bottom => ReplacingVisual.Bottom;

        String? IVisualElement.Class => ReplacingVisual.Class;

        IAppliedStyle? IVisualElement.Style => ReplacingVisual.Style;

        Boolean IVisualElement.IsClipsContent
        {
            get => ReplacingVisual.IsClipsContent;
            set => ReplacingVisual.IsClipsContent = value;
        }

        public virtual void OnParentChanging(IVisualElement? newParent)
        {
            //ReplacingVisual.OnParentChanging(newParent);
            if (newParent == null)
                _viewGroup.RemoveView(this);
            else _viewGroup.AddView(this);
        }

        event Action<IVisualElement>? IVisualElement.Disposed
        {
            add => ReplacingVisual.Disposed += value;
            remove => ReplacingVisual.Disposed -= value;
        }

        void IVisualElement.RaisePropertyChanged(String propertyName,
                                                 Object? value)
        {
            ReplacingVisual.RaisePropertyChanged(propertyName, value);
        }

        QuantifiedDouble? IVisualElement.Width
        {
            get => ReplacingVisual.Width;
            set => ReplacingVisual.Width = value;
        }

        QuantifiedDouble? IVisualElement.Height
        {
            get => ReplacingVisual.Height;
            set => ReplacingVisual.Height = value;
        }

        HorizontalAlignments IVisualElement.HorizontalAlignment
        {
            get => ReplacingVisual.HorizontalAlignment;
            set => ReplacingVisual.HorizontalAlignment = value;
        }

        VerticalAlignments IVisualElement.VerticalAlignment
        {
            get => ReplacingVisual.VerticalAlignment;
            set => ReplacingVisual.VerticalAlignment = value;
        }

        IBrush? IVisualElement.Background
        {
            get => ReplacingVisual.Background;
            set => ReplacingVisual.Background = value;
        }

        QuantifiedThickness IVisualElement.Margin
        {
            get => ReplacingVisual.Margin;
            set => ReplacingVisual.Margin = value;
        }

        Double IVisualElement.Opacity => ReplacingVisual.Opacity;

        Visibility IVisualElement.Visibility
        {
            get => ReplacingVisual.Visibility;
            set => ReplacingVisual.Visibility = value;
        }

        QuantifiedThickness IVisualElement.BorderRadius
        {
            get => ReplacingVisual.BorderRadius;
            set => ReplacingVisual.BorderRadius = value;
        }

        VisualBorder IVisualElement.Border
        {
            get => ReplacingVisual.Border;
            set => ReplacingVisual.Border = value;
        }

        Boolean IVisualElement.IsEnabled
        {
            get => ReplacingVisual.IsEnabled;
            set => ReplacingVisual.IsEnabled = value;
        }

        TransformationMatrix IVisualElement.Transform
        {
            get => ReplacingVisual.Transform;
            set => ReplacingVisual.Transform = value;
        }

        Boolean IVisualElement.TryGetDependencyProperty(DeclarationProperty declarationProperty,
                                                        out IDependencyProperty dependencyProperty)
        {
            return ReplacingVisual.TryGetDependencyProperty(declarationProperty, out dependencyProperty);
        }

        ILabel? IVisualElement.BeforeLabel
        {
            get => ReplacingVisual.BeforeLabel;
            set => ReplacingVisual.BeforeLabel = value;
        }

        ILabel? IVisualElement.AfterLabel
        {
            get => ReplacingVisual.AfterLabel;
            set => ReplacingVisual.AfterLabel = value;
        }

        Int32 IVisualElement.ZIndex => ReplacingVisual.ZIndex;

        IBoxShadow IVisualElement.BoxShadow => ReplacingVisual.BoxShadow;

        public IVisualElement ReplacingVisual { get; }

        public View NativeView { get; }
        //{
        //    get => _nativeView;
        //    //set => SetNativeView(value);
        //}

        public sealed override void AddView(View? child)
        {
            base.AddView(child);
        }

        public override Boolean OnInterceptTouchEvent(MotionEvent? ev)
        {
            System.Diagnostics.Debug.WriteLine("intercept touch: " + ev);
            return base.OnInterceptTouchEvent(ev);
        }

        //private void SetNativeView(View? value)
        //{
        //    if (_nativeView != null)
        //        RemoveView(_nativeView);

        //    _nativeView = value;
        //    if (value != null)
        //        AddView(value);
        //}

        //protected override void OnLayout(Boolean changed,
        //                                 Int32 l,
        //                                 Int32 t,
        //                                 Int32 r,
        //                                 Int32 b)
        //{
        //    NativeView.Layout(l, t, r, b);
        //}

        protected readonly ViewGroup _viewGroup;
    }
}

using System;

using System.Threading;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Mvvm;
using Das.Views.Rendering;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;

#endif

namespace Das.Views
{
    public abstract class VisualElement : NotifyPropertyChangedBase,
                                          IVisualElement
    {
        protected VisualElement(IVisualBootstrapper visualBootstrapper)
        {
            _visualBootstrapper = visualBootstrapper;
            Id = Interlocked.Increment(ref _currentId);
        }

        protected Boolean TryGetSize(out ValueSize size)
        {
            var width = Width;
            if (width == null || width == 0)
            {
                size = ValueSize.Empty;
                return false;
            }

            var height = Height;
            if (height == null || height == 0)
            {
                size = ValueSize.Empty;
                return false;
            }

            size = new ValueSize(width.Value, height.Value);
            return true;
        }

        public virtual ValueSize Measure(IRenderSize availableSpace,
                                         IMeasureContext measureContext)
        {
            TryGetSize(out var size);
            return size;

            //var width = Width;
            //if (width == null || width == 0)
            //    return ValueSize.Empty;
            //var height = Height;
            //if (height == null || height == 0)
            //    return ValueSize.Empty;

            //return new ValueSize(width.Value, height.Value);

            //return measureContext.GetStyleDesiredSize(this);
        }

        public virtual void Arrange(IRenderSize availableSpace,
                                    IRenderContext renderContext)
        {
            if (TryGetSize(out var size))
                renderContext.DrawContentElement(this, size);
        }

        public virtual void InvalidateMeasure()
        {
            //System.Diagnostics.Debug.WriteLine("InvalidateMeasure " + this);

            IsRequiresMeasure = true;
            IsRequiresArrange = true;
        }

        public virtual void InvalidateArrange()
        {
            IsRequiresArrange = true;
        }

        public virtual Boolean IsRequiresMeasure
        {
            get => _isRequiresMeasure;
            protected set => SetValue(ref _isRequiresMeasure, value);
                    //OnIsRequiresMeasureChanging, OnIsRequiresMeasureChanged);
        }

        public virtual Boolean IsRequiresArrange
        {
            get
            {
                return _isRequiresArrange;
            }
            protected set => SetValue(ref _isRequiresArrange, value);
                    //OnIsRequiresArrangeChanging, OnIsRequiresArrangeChangedAsync);
        }


        public virtual Int32 Id { get; private set; }

        public virtual Boolean IsClipsContent { get; set; }

        public event Action<IVisualElement>? Disposed;

        public virtual IControlTemplate? Template
        {
            get => _template;
            // ReSharper disable once UnusedMember.Global
            set => SetValue(ref _template, value);
        }

        public virtual void AcceptChanges(ChangeType changeType)
        {
            switch (changeType)
            {
                case ChangeType.Measure:
                    //lock (_measureLock)
                        IsRequiresMeasure = false;
                    break;

                case ChangeType.Arrange:
                    //lock (_arrangeLock)
                        IsRequiresArrange = false;
                    break;
            }
        }

        public new void RaisePropertyChanged(String propertyName,
                                             Object? value)
        {
            base.RaisePropertyChanged(propertyName, value);
        }

        public virtual void OnParentChanging(IVisualElement? newParent)
        {
        }

        public override void Dispose()
        {
            base.Dispose();

            Disposed?.Invoke(this);

            Disposed = null;
        }

        public Boolean Equals(IVisualElement other)
        {
            return ReferenceEquals(this, other);
        }

        public Double? Width
        {
            get => WidthProperty.GetValue(this);
            set => WidthProperty.SetValue(this, value);
        }

        public Double? Height
        {
            get => HeightProperty.GetValue(this);
            set => HeightProperty.SetValue(this, value);
        }

        public HorizontalAlignments HorizontalAlignment
        {
            get => HorizontalAlignmentProperty.GetValue(this);
            set => HorizontalAlignmentProperty.SetValue(this, value);
        }

        public VerticalAlignments VerticalAlignment
        {
            get => VerticalAlignmentProperty.GetValue(this);
            set => VerticalAlignmentProperty.SetValue(this, value);
        }

        public IBrush? Background
        {
            get => BackgroundProperty.GetValue(this);
            set => BackgroundProperty.SetValue(this, value);
        }

        public Thickness? Margin
        {
            get => MarginProperty.GetValue(this);
            set => MarginProperty.SetValue(this, value);
        }

        // ReSharper disable once UnusedMember.Global
        public virtual Boolean IsEnabled
        {
            get => _isEnabled;
            set => SetValue(ref _isEnabled, value);
        }

        //protected virtual Task OnIsRequiresArrangeChangedAsync(Boolean value)
        //{
        //    return TaskEx.CompletedTask;
        //}

        //protected virtual Boolean OnIsRequiresArrangeChanging(Boolean oldValue,
        //                                                      Boolean newValue)
        //{
        //    return true;
        //}

        //protected virtual void OnIsRequiresMeasureChanged(Boolean value)
        //{
            
        //}

        //protected virtual Boolean OnIsRequiresMeasureChanging(Boolean oldValue,
        //                                                      Boolean newValue)
        //{
        //    return true;
        //}

        public override String ToString()
        {
            return GetType().Name + " req arrange: " + IsRequiresArrange + 
                   " measure: " + IsRequiresMeasure;
        }

        private static Int32 _currentId;


        public static readonly DependencyProperty<IVisualElement, Double?> WidthProperty =
            DependencyProperty<IVisualElement, Double?>.Register(nameof(Width), null);

        public static readonly DependencyProperty<IVisualElement, Double?> HeightProperty =
            DependencyProperty<IVisualElement, Double?>.Register(nameof(Height), null);


        public static readonly DependencyProperty<IVisualElement, VerticalAlignments> VerticalAlignmentProperty =
            DependencyProperty<IVisualElement, VerticalAlignments>.Register(nameof(VerticalAlignment),
                VerticalAlignments.Default);

        public static readonly DependencyProperty<IVisualElement, HorizontalAlignments> HorizontalAlignmentProperty =
            DependencyProperty<IVisualElement, HorizontalAlignments>.Register(nameof(HorizontalAlignment),
                HorizontalAlignments.Default);

        public static readonly DependencyProperty<IVisualElement, IBrush?> BackgroundProperty =
            DependencyProperty<IVisualElement, IBrush?>.Register(nameof(Background), default);

        public static readonly DependencyProperty<IVisualElement, Thickness?> MarginProperty =
            DependencyProperty<IVisualElement, Thickness?>.Register(nameof(Margin),
                null);

        protected readonly IVisualBootstrapper _visualBootstrapper;


        private Boolean _isEnabled;


        private Boolean _isRequiresArrange;
        private Boolean _isRequiresMeasure;

        private IControlTemplate? _template;
    }
}
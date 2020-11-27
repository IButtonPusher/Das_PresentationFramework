using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Mvvm;
using Das.Views.Panels;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;

#endif

namespace Das.Views.Rendering
{
    public abstract class VisualElement : NotifyPropertyChangedBase,
                                          IVisualElement
    {
        protected VisualElement(IVisualBootStrapper visualBootStrapper)
        {
            _visualBootStrapper = visualBootStrapper;
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
            protected set => SetValue(ref _isRequiresMeasure, value,
                    OnIsRequiresMeasureChanging, OnIsRequiresMeasureChangedAsync);
        }

        public virtual Boolean IsRequiresArrange
        {
            get => _isRequiresArrange;
            protected set
            {
                if (value)
                {}

                SetValue(ref _isRequiresArrange, value,
                    OnIsRequiresArrangeChanging, OnIsRequiresArrangeChangedAsync);
            }
        }

        //public Boolean IsRequiresMeasure { get; }

        //public Boolean IsRequiresArrange { get; }


       

        public virtual IVisualElement DeepCopy()
        {
            var newObject = _visualBootStrapper.Instantiate<VisualElement>(GetType());
            //var newObject = (VisualElement) Activator.CreateInstance(GetType());
            newObject.Id = Id;
            return newObject;
        }

        public virtual Int32 Id { get; private set; }

        public virtual Boolean IsClipsContent { get; set; }

        public event Action<IVisualElement>? Disposed;

        public virtual IControlTemplate? Template
        {
            get => _template;
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

        public new void RaisePropertyChanged(String propertyName)
        {
            base.RaisePropertyChanged(propertyName);
        }

        public virtual void OnParentChanging(IContainerVisual? newParent)
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

        // ReSharper disable once UnusedMember.Global
        public virtual Boolean IsEnabled
        {
            get => _isEnabled;
            set => SetValue(ref _isEnabled, value);
        }

        protected virtual Task OnIsRequiresArrangeChangedAsync(Boolean value)
        {
            return TaskEx.CompletedTask;
        }

        protected virtual Boolean OnIsRequiresArrangeChanging(Boolean oldValue,
                                                              Boolean newValue)
        {
            //if (newValue)
            //    Debug.WriteLine(this + " will require arrange");
            return true;
        }

        protected virtual Task OnIsRequiresMeasureChangedAsync(Boolean value)
        {
            if (value)
            {
                //Debug.WriteLine("item " + this + " set to needs measure");
            }

            return TaskEx.CompletedTask;
        }

        protected virtual Boolean OnIsRequiresMeasureChanging(Boolean oldValue,
                                                              Boolean newValue)
        {
            return true;
        }

        private static Int32 _currentId;


        public static readonly DependencyProperty<IVisualElement, Double?> WidthProperty =
            DependencyProperty<IVisualElement, Double?>.Register(nameof(Width), null);

        public static readonly DependencyProperty<IVisualElement, Double?> HeightProperty =
            DependencyProperty<IVisualElement, Double?>.Register(nameof(Height), null);


        public static readonly DependencyProperty<IVisualElement, VerticalAlignments> VerticalAlignmentProperty =
            DependencyProperty<IVisualElement, VerticalAlignments>.Register(nameof(VerticalAlignment),
                VerticalAlignments.Stretch);

        public static readonly DependencyProperty<IVisualElement, HorizontalAlignments> HorizontalAlignmentProperty =
            DependencyProperty<IVisualElement, HorizontalAlignments>.Register(nameof(HorizontalAlignment),
                HorizontalAlignments.Stretch);

        public static readonly DependencyProperty<IVisualElement, IBrush?> BackgroundProperty =
            DependencyProperty<IVisualElement, IBrush?>.Register(nameof(Background), default);

        private readonly IVisualBootStrapper _visualBootStrapper;


        private Boolean _isEnabled;


        private Boolean _isRequiresArrange;
        private Boolean _isRequiresMeasure;

        private IControlTemplate? _template;
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Core.Geometry;
using Das.Views.Mvvm;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Styles.Application;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;

#endif

namespace Das.Views
{
    public abstract partial class VisualElement : NotifyPropertyChangedBase,
                                                  IVisualElement
    {
        protected VisualElement(IVisualBootstrapper visualBootstrapper)
        {
            _visualBootstrapper = visualBootstrapper;
            _arrangedBounds = ValueRenderRectangle.Empty;
            
            _measuredSize = ValueSize.Empty;
            Id = Interlocked.Increment(ref _currentId);
        }

        public virtual ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
                                                      IMeasureContext measureContext)
            where TRenderSize : IRenderSize
        {
            measureContext.TryGetElementSize(this, availableSpace, out _measuredSize);
            return _measuredSize;
        }


        private ValueRenderRectangle _arrangedBounds;

        public virtual ValueRenderRectangle ArrangedBounds
        {
            get => _arrangedBounds;
            set => SetValue(ref _arrangedBounds, value);
        }

        public virtual void Arrange<TRenderSize>(TRenderSize availableSpace,
                                                 IRenderContext renderContext)
            where TRenderSize : IRenderSize
        {
           //intentionally left blank
        }

        public virtual void InvalidateMeasure()
        {
            if (_isLayoutSuspended)
                return;

            IsRequiresMeasure = true;
            IsRequiresArrange = true;
            _visualBootstrapper.LayoutQueue.QueueVisualForMeasure(this);
        }

        public virtual void InvalidateArrange()
        {
            if (_isLayoutSuspended)
                return;

            IsRequiresArrange = true;
            _visualBootstrapper.LayoutQueue.QueueVisualForArrange(this);
        }

        public virtual void AcceptChanges(ChangeType changeType)
        {
            switch (changeType)
            {
                case ChangeType.Measure:
                    IsRequiresMeasure = false;
                    break;

                case ChangeType.Arrange:
                    IsRequiresArrange = false;
                    break;
            }
        }

        public new void RaisePropertyChanged(String propertyName,
                                             Object? value)
        {
            base.RaisePropertyChanged(propertyName, value);
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public IAppliedStyle? Style { get; set; }

        public virtual void OnParentChanging(IVisualElement? newParent)
        {
        }

        public override void Dispose()
        {
            base.Dispose();

            if (BeforeLabel is { } beforeLabel)
            {
                beforeLabel.Dispose();
                BeforeLabel = default;
            }

            if (AfterLabel is { } afterLabel)
            {
                afterLabel.Dispose();
                AfterLabel = default;
            }

            Disposed?.Invoke(this);

            Disposed = null;
        }

        public Boolean Equals(IVisualElement other)
        {
            return ReferenceEquals(this, other);
        }

        public override String ToString()
        {
            return GetType().Name;
        }

       
        private static Int32 _currentId;
        private ValueSize _measuredSize;
        private Boolean _isLayoutSuspended;


        protected readonly IVisualBootstrapper _visualBootstrapper;

        protected virtual void OnTemplateSet(IVisualTemplate? newValue)
        {
            
        }

        public virtual void SuspendLayout()
        {
            _isLayoutSuspended = true;
        }

        public virtual void ResumeLayout()
        {
            _isLayoutSuspended = false;
        }
    }
}
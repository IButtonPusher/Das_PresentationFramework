using System;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Core.Geometry;
using Das.Views.Mvvm;
using Das.Views.Rendering;
using Das.Views.Styles;
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
            
            _measuredSize = ValueSize.Empty;
            Id = Interlocked.Increment(ref _currentId);
        }

        public virtual ValueSize Measure(IRenderSize availableSpace,
                                         IMeasureContext measureContext)
        {
            measureContext.TryGetElementSize(this, out _measuredSize);
            return _measuredSize;
        }

        public virtual void Arrange(IRenderSize availableSpace,
                                    IRenderContext renderContext)
        {
            //if (_measuredSize.IsEmpty)
            //    return;

            //var letsUse = _measuredSize.LeastCommonDenominator(availableSpace);

            //if (letsUse.IsEmpty)
            //    return;

            //renderContext.DrawContentElement(this, letsUse);
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
        public IStyleSheet? Style { get; set; }

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

        public virtual Boolean IsMarkupNameAlias(String markupTag)
        {
            return false;
        }


        public override String ToString()
        {
            return GetType().Name;
            //+ " req arrange: " + IsRequiresArrange +
            //" measure: " + IsRequiresMeasure;
        }

       
        private static Int32 _currentId;
        private ValueSize _measuredSize;


        protected readonly IVisualBootstrapper _visualBootstrapper;

        protected virtual void OnTemplateSet(IVisualTemplate? newValue)
        {
            
        }
    }
}
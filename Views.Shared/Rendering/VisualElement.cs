using System;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Controls;
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
        private readonly IVisualBootStrapper _templateResolver;

        protected VisualElement(IVisualBootStrapper templateResolver)
        {
            _templateResolver = templateResolver;
            Id = Interlocked.Increment(ref _currentId);
        }

        public virtual ValueSize Measure(IRenderSize availableSpace,
                                         IMeasureContext measureContext)
        {
            return measureContext.GetStyleDesiredSize(this);
        }

        public virtual void InvalidateMeasure()
        {
            IsRequiresMeasure = true;
        }

        public virtual void InvalidateArrange()
        {
            IsRequiresArrange = true;
        }


        private Boolean _isRequiresMeasure;

        public virtual Boolean IsRequiresMeasure
        {
            get => _isRequiresMeasure;
            protected set => SetValue(ref _isRequiresMeasure, value,
                OnIsRequiresMeasureChanging, OnIsRequiresMeasureChangedAsync);
        }

        protected virtual Task OnIsRequiresMeasureChangedAsync(Boolean value)
        {
            return TaskEx.CompletedTask;
        }

        protected virtual Boolean OnIsRequiresMeasureChanging(Boolean oldValue, 
                                                              Boolean newValue)
        {
            return true;
        }


        private Boolean _isRequiresArrange;

        public virtual Boolean IsRequiresArrange
        {
            get => _isRequiresArrange;
            protected set => SetValue(ref _isRequiresArrange, value,
                OnIsRequiresArrangeChanging, OnIsRequiresArrangeChangedAsync);
        }

        protected virtual Task OnIsRequiresArrangeChangedAsync(Boolean value)
        {
            return TaskEx.CompletedTask;
        }

        protected virtual Boolean OnIsRequiresArrangeChanging(Boolean oldValue, 
                                                    Boolean newValue)
        {
            return true;
        }

        //public Boolean IsRequiresMeasure { get; }

        //public Boolean IsRequiresArrange { get; }


        public abstract void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext);

        public virtual IVisualElement DeepCopy()
        {
            var newObject = _templateResolver.Instantiate<VisualElement>(GetType());
            //var newObject = (VisualElement) Activator.CreateInstance(GetType());
            newObject.Id = Id;
            return newObject;
        }

        public virtual Int32 Id { get; private set; }

        public event Action<IVisualElement>? Disposed;


        private IControlTemplate? _template;

        public virtual IControlTemplate? Template
        {
            get => _template;
            set => SetValue(ref _template, value);
        }

        public void AcceptChanges(ChangeType changeType)
        {
            switch (changeType)
            {
                case ChangeType.Measure:
                    _isRequiresMeasure = false;
                    break;

                case ChangeType.Arrange:
                    _isRequiresArrange = false;
                    break;
            }
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

        // ReSharper disable once UnusedMember.Global
        public virtual Boolean IsEnabled
        {
            get => _isEnabled;
            set => SetValue(ref _isEnabled, value);
        }

        private static Int32 _currentId;


        private Boolean _isEnabled;
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Mvvm;

namespace Das.Views.Rendering
{
    public abstract class VisualElement : NotifyPropertyChangedBase,
                                          IVisualElement
    {
        protected VisualElement()
        {
            Id = Interlocked.Increment(ref _currentId);
        }

        public abstract ISize Measure(ISize availableSpace,
                                      IMeasureContext measureContext);

        public abstract void Arrange(ISize availableSpace,
                                     IRenderContext renderContext);

        public virtual IVisualElement DeepCopy()
        {
            var newObject = (VisualElement) Activator.CreateInstance(GetType());
            newObject.Id = Id;
            return newObject;
        }

        public Int32 Id { get; private set; }

        public event Action<IVisualElement>? Disposed;

        public override void Dispose()
        {
            base.Dispose();

            Disposed?.Invoke(this);

            Disposed = null;
        }

        public virtual Boolean IsEnabled
        {
            get => _isEnabled;
            set => SetValue(ref _isEnabled, value);
        }

        private static Int32 _currentId;


        private Boolean _isEnabled;
    }
}
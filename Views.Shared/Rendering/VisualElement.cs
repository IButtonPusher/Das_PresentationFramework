using System;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public abstract class VisualElement : IVisualElement
    {
        protected VisualElement()
        {
            Id = Interlocked.Increment(ref _currentId);
        }

        public abstract ISize Measure(ISize availableSpace, IMeasureContext measureContext);

        public abstract void Arrange(ISize availableSpace, IRenderContext renderContext);

        public virtual IVisualElement DeepCopy()
        {
            var newObject = (VisualElement) Activator.CreateInstance(GetType());
            newObject.Id = Id;
            return newObject;
        }

        public Int32 Id { get; private set; }

        public abstract void Dispose();

        private static Int32 _currentId;
    }
}
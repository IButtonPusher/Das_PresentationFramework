using System;
using System.Threading;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public abstract class VisualElement : IVisualElement
    {
        private static Int32 _currentId;

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

        public int Id { get; private set; }
    }
}
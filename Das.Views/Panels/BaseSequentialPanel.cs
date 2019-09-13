using System;
using System.Collections.Generic;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public abstract class BaseSequentialPanel<T> : BasePanel<T>, ISequentialPanel
    {
        public Orientations Orientation { get; set; }

        protected abstract IEnumerable<IVisualElement> GetChildrenToRender();

        private readonly ISequentialRenderer _renderer;

        protected BaseSequentialPanel(IDataBinding<T> binding,
            ISequentialRenderer renderer = null) : base(binding)
        {
            _renderer = EnsureRenderer(renderer);
        }

        protected BaseSequentialPanel(ISequentialRenderer renderer = null)
            : this(null, renderer)
        {
        }

        private static ISequentialRenderer EnsureRenderer(ISequentialRenderer input)
            => input ?? new SequentialRenderer();

        public override ISize Measure(ISize availableSpace, IMeasureContext measureContext)
            => _renderer.Measure(this, GetChildrenToRender(), Orientation,
                availableSpace, measureContext);

        public override void Arrange(ISize availableSpace, IRenderContext renderContext)
            => _renderer.Arrange(Orientation, availableSpace, renderContext);
    }
}
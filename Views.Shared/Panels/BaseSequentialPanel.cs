using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public abstract class BaseSequentialPanel<T> : BasePanel<T>, ISequentialPanel
    {
        protected BaseSequentialPanel(IDataBinding<T>? binding,
                                      ISequentialRenderer? renderer = null)
            : base(binding)
        {
            _renderer = EnsureRenderer(renderer);
        }

        protected BaseSequentialPanel(ISequentialRenderer? renderer = null)
            : this(null, renderer)
        {
        }

        public Orientations Orientation { get; set; }

        public override ISize Measure(IRenderSize availableSpace, 
                                      IMeasureContext measureContext)
        {
            return _renderer.Measure(this, GetChildrenToRender(), Orientation,
                availableSpace, measureContext);
        }

        public override void Arrange(IRenderSize availableSpace, 
                                     IRenderContext renderContext)
        {
            _renderer.Arrange(Orientation, availableSpace, renderContext);
        }

        private static ISequentialRenderer EnsureRenderer(ISequentialRenderer? input)
        {
            return input ?? new SequentialRenderer();
        }

        protected abstract IEnumerable<IVisualElement> GetChildrenToRender();

        private readonly ISequentialRenderer _renderer;
    }
}
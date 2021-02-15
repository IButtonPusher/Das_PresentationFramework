using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public abstract class BaseSurrogatedVisual : VisualElement
    {
        protected BaseSurrogatedVisual(IVisualBootstrapper visualBootstrapper)
            : base(visualBootstrapper)
        //: base(NullVisualBootStrapper.Instance)
        {
        }

        public IVisualElement? Parent
        {
            get => _parent;
            set => SetValue(ref _parent, value);
        }

        public override void Arrange<TRenderSize>(TRenderSize availableSpace,
                                                  IRenderContext renderContext)
        {
            throw new NotSupportedException("A surrogate is required for this control");
        }

        public override ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
                                                       IMeasureContext measureContext)
        {
            throw new NotSupportedException("A surrogate is required for this control");
        }

        public override void OnParentChanging(IVisualElement? newParent)
        {
            base.OnParentChanging(newParent);
            Parent = newParent;
        }

        private IVisualElement? _parent;
    }
}

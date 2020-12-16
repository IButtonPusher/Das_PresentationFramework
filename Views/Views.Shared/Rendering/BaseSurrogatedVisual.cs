using System;
using Das.Views.Panels;
using Das.Views.Templates;

namespace Das.Views.Rendering
{
    public abstract class BaseSurrogatedVisual : VisualElement
    {
        public override void OnParentChanging(IVisualElement? newParent)
        {
            base.OnParentChanging(newParent);
            Parent = newParent;
        }

        private IVisualElement? _parent;

        public IVisualElement? Parent
        {
            get => _parent;
            set => SetValue(ref _parent, value);
        }

        protected BaseSurrogatedVisual(IVisualBootstrapper visualBootstrapper) 
            : base(visualBootstrapper)
            //: base(NullVisualBootStrapper.Instance)
        {
        }
    }
}

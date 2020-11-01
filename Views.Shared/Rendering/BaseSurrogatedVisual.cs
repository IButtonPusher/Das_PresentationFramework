using System;
using Das.Views.Panels;

namespace Das.Views.Rendering
{
    public abstract class BaseSurrogatedVisual : VisualElement
    {
        public override void OnParentChanging(IContentContainer? newParent)
        {
            base.OnParentChanging(newParent);
            Parent = newParent;
        }

        private IContentContainer? _parent;

        public IContentContainer? Parent
        {
            get => _parent;
            set => SetValue(ref _parent, value);
        }   
    }
}

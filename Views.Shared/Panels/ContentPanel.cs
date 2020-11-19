using System;
using Das.Views.DataBinding;

namespace Das.Views.Panels
{
    public class ContentPanel : ContentPanel<Object>
    {
        public ContentPanel(IVisualBootStrapper templateResolver) : base(templateResolver)
        {
        }

        public ContentPanel(IVisualBootStrapper templateResolver, IDataBinding<Object> binding) : base(templateResolver, binding)
        {
        }
    }
}

//    public class ContentPanel : VisualElement,
//                                IContentContainer, 
//                                IVisualContainer
//    {
//        public virtual IVisualElement? Content
//        {
//            get => _content;
//            set => SetValue(ref _content, value, 
//                OnContentChanging, OnContentChanged);
//        }

//        protected virtual Boolean OnContentChanging(IVisualElement? oldValue,
//                                                    IVisualElement? newValue)
//        {
//            return ContentPanelHelper.OnContentChanging(this, oldValue, newValue);
//        }

//        public override ISize Measure(IRenderSize availableSpace,
//                                      IMeasureContext measureContext)
//        {
//            return ContentPanelHelper.Measure(this, availableSpace, measureContext);
//        }

//        public override void Arrange(IRenderSize availableSpace,
//                                     IRenderContext renderContext)
//        {
//            ContentPanelHelper.Arrange(this, availableSpace, renderContext);
//        }

//        protected virtual void OnContentChanged(IVisualElement? obj)
//        {
//            IsChanged = true;
//        }

//        public virtual Boolean IsChanged
//        {
//            get => _isChanged || Content is IChangeTracking {IsChanged: true} ct;
//            protected set => SetValue(ref _isChanged, value);
//        }

//        public virtual void AcceptChanges()
//        {
//            IsChanged = false;
//            if (Content is IChangeTracking ct)
//                ct.AcceptChanges();
//        }

//        private IVisualElement? _content;
//        private Boolean _isChanged;
//    }
//}

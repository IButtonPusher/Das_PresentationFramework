using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public abstract class ContentPanel<T> : BindableElement<T>,
                                            IContentContainer, 
                                            IVisualContainer
    {
        protected ContentPanel()
        {
        }

        protected ContentPanel(IDataBinding<T> binding) : base(binding)
        {
        }


        public virtual IVisualElement? Content
        {
            get => _content;
            set => SetValue(ref _content, value, 
                OnContentChanging, OnContentChanged);
        }

        protected virtual void OnContentChanged(IVisualElement? obj)
        {
            IsChanged = true;
        }

        protected virtual Boolean OnContentChanging(IVisualElement? oldValue, 
                                          IVisualElement? newValue)
        {
            if (oldValue is {} old)
                old.OnParentChanging(null);

            if (newValue is {} valid)
                valid.OnParentChanging(this);

            return true;
        }

        public override ISize Measure(ISize availableSpace,
                                      IMeasureContext measureContext)
        {
            return Content?.Measure(availableSpace, measureContext) ?? Size.Empty;
        }


        public override void Arrange(ISize availableSpace,
                                     IRenderContext renderContext)
        {
            var content = Content;
            content?.Arrange(availableSpace, renderContext);
        }

        public override IVisualElement DeepCopy()
        {
            var newObject = (IContentContainer) base.DeepCopy();

            var content = Content;
            if (content == null)
                return newObject;

            var newContent = content.DeepCopy();
            newObject.Content = newContent;
            if (newObject is BindableElement<T> bindable)
                bindable.Binding = Binding;

            return newObject;
        }

        public virtual void AcceptChanges()
        {
            IsChanged = false;
            if (Content is IChangeTracking ct)
                ct.AcceptChanges();
        }

        public virtual Boolean IsChanged
        {
            get => _isChanged || Content is IChangeTracking ct && ct.IsChanged;
            protected set => SetValue(ref _isChanged, value);
        }

        IList<IVisualElement> IVisualContainer.Children
        {
            get
            {
                var content = Content;
                if (content == null)
                    return new List<IVisualElement>();
                return new List<IVisualElement> {content};
            }
        }

        void IVisualContainer.AddChild(IVisualElement element)
        {
            if (Content != null)
                throw new Exception("Content is already set");

            Content = element;
        }

        void IVisualContainer.AddChildren(params IVisualElement[] elements)
        {
            throw new NotSupportedException();
        }

        public void OnChildDeserialized(IVisualElement element, INode node)
        {
        }

        public virtual Boolean Contains(IVisualElement element)
        {
            if (Content == element)
                return true;

            if (Content is IVisualContainer container)
                return container.Contains(element);

            return false;
        }

        public override void SetDataContext(Object? dataContext)
        {
            IsChanged = true;
            DataContext = dataContext;

            if (Content is IBindableElement bindable)
                bindable.SetDataContext(dataContext);
        }

        public override async Task SetDataContextAsync(Object? dataContext)
        {
            IsChanged = true;
            DataContext = dataContext;

            if (Content is IBindableElement bindable)
                await bindable.SetDataContextAsync(dataContext);
        }


        public override void SetBoundValue(T value)
        {
            IsChanged = true;
            Binding = new ObjectBinding<T>(value);

            if (Content is IBindableElement<T> bindable)
                bindable.SetBoundValue(value);
            else if (Content is IBindableElement almost)
                almost.SetDataContext(value);
        }

        public override async Task SetBoundValueAsync(T value)
        {
            IsChanged = true;
            Binding = new ObjectBinding<T>(value);

            if (Content is IBindableElement<T> bindable)
                await bindable.SetBoundValueAsync(value);
            else if (Content is IBindableElement almost)
                await almost.SetDataContextAsync(value);
        }

        private IVisualElement? _content;


        private Boolean _isChanged;
    }
}
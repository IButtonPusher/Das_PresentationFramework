using System;
using System.Collections.Generic;
using Das.Serializer;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public abstract class ContentPanel<T> : BindableElement<T>,
        IContentContainer, IVisualContainer
    {
        protected ContentPanel()
        {
        }

        protected ContentPanel(IDataBinding<T> binding) : base(binding)
        {
        }

        private IVisualElement _content;

        public virtual IVisualElement Content
        {
            get => _content;
            set
            {
                _content = value;
                IsChanged = true;
            }
        }

        public override ISize Measure(ISize availableSpace, IMeasureContext measureContext)
            => Content?.Measure(availableSpace, measureContext) ?? Size.Empty;


        public override void Arrange(ISize availableSpace, IRenderContext renderContext)
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
            => throw new NotSupportedException();

        public virtual void OnChildDeserialized(IVisualElement element, INode node)
        {
        }

        public virtual bool Contains(IVisualElement element)
        {
            if (Content == element)
                return true;

            if (Content is IVisualContainer container)
                return container.Contains(element);

            return false;
        }


        public override void SetDataContext(object dataContext)
        {
            IsChanged = true;
            DataContext = dataContext;

            if (Content is IBindableElement bindable)
                bindable.SetDataContext(dataContext);
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

        public virtual void AcceptChanges()
        {
            IsChanged = false;
        }

        public virtual bool IsChanged { get; private set; }
    }
}
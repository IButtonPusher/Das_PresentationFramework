using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Defaults;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Styles;

namespace Das.Views.Panels
{
    public abstract class ContentPanel<T> : BaseContainerVisual<T>,
                                            IContentContainer,
                                            IVisualContainer,
                                            IContentPresenter
    {
        protected ContentPanel(IVisualBootStrapper templateResolver)
            : base(templateResolver)
        {
            _contentTemplate = new DefaultContentTemplate(templateResolver, this);
        }

        protected ContentPanel(IVisualBootStrapper templateResolver,
                               IDataBinding<T> binding)
            : base(templateResolver, binding)
        {
            _contentTemplate = new DefaultContentTemplate(templateResolver, this);
        }


        public virtual IVisualElement? Content
        {
            get => _content;
            set => SetValue(ref _content, value,
                OnContentChanging, OnContentChanged);
        }

        public override ValueSize Measure(IRenderSize availableSpace,
                                          IMeasureContext measureContext)
        {
            var content = GetContent();
            if (content == null)
                return ValueSize.Empty;

            var padding = measureContext.GetStyleSetter<Thickness>(StyleSetter.Padding, this);
            var res = measureContext.MeasureElement(content, availableSpace);

            IsRequiresMeasure = false;

            return padding.IsEmpty ? res : res + padding;
        }

        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
            var content = GetContent();
            if (content == null) 
                return;

            var padding = renderContext.GetStyleSetter<Thickness>(StyleSetter.Padding, this);
            var contentMargin = renderContext.GetStyleSetter<Thickness>(StyleSetter.Margin, content);

            var target = new ValueRenderRectangle(
                padding.Left + contentMargin.Left,
                padding.Top + contentMargin.Top,
                availableSpace.Width - padding.Width - contentMargin.Right,
                availableSpace.Height - padding.Height - contentMargin.Right,
                availableSpace.Offset);

            //var target = new ValueRenderRectangle(padding.Left, padding.Top,
            //        availableSpace.Width - padding.Width,
            //        availableSpace.Height - padding.Height,
            //        availableSpace.Offset);

            renderContext.DrawElement(content, target);
            IsRequiresArrange = false;
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

        public override void AcceptChanges()
        {
            IsChanged = false;
            if (Content is IChangeTracking ct)
                ct.AcceptChanges();
        }


        public IDataTemplate? ContentTemplate
        {
            get => _contentTemplate;
            set => SetValue(ref _contentTemplate,
                value ?? _contentTemplate);
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
            DataContext = dataContext;

            if (Content is IBindableElement bindable)
                bindable.SetDataContext(dataContext);

            IsChanged = true;
        }

        public override async Task SetDataContextAsync(Object? dataContext)
        {
            DataContext = dataContext;

            if (Content is IBindableElement bindable)
                await bindable.SetDataContextAsync(dataContext);

            IsChanged = true;
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

        protected IVisualElement? GetContent()
        {
            if (Content is { } content)
                return content;

            if (!(Value is { } valid))
                return null;

            return Content = _contentTemplate.BuildVisual(valid);
        }

        protected virtual void OnContentChanged(IVisualElement? obj)
        {
            IsChanged = true;
        }

        protected virtual Boolean OnContentChanging(IVisualElement? oldValue,
                                                    IVisualElement? newValue)
        {
            if (oldValue is { } old)
            {
                if (old is INotifyPropertyChanged notifier)
                    notifier.PropertyChanged -= OnChildPropertyChanged;
                old.OnParentChanging(null);
            }

            if (newValue is { } valid)
            {
                if (valid is INotifyPropertyChanged notifier)
                    notifier.PropertyChanged += OnChildPropertyChanged;
                valid.OnParentChanging(this);
            }

            return true;
        }

        //private void OnContentPropertyChanged(Object sender,
        //                                      PropertyChangedEventArgs e)
        //{
        //    switch (e.PropertyName)
        //    {
        //        case nameof(IsChanged) when Content is IChangeTracking content && content.IsChanged:
        //            IsChanged = true;
        //            break;

        //        case nameof(IsRequiresMeasure) when Content is IVisualRenderer renderer && 
        //            renderer.IsRequiresMeasure:
        //            IsRequiresMeasure = true;
        //            break;

        //        case nameof(IsRequiresArrange) when Content is IVisualRenderer renderer && 
        //                                            renderer.IsRequiresArrange:
        //            IsRequiresArrange = true;
        //            break;
        //    }
        //}

        private IVisualElement? _content;
        private IDataTemplate _contentTemplate;

    }
}
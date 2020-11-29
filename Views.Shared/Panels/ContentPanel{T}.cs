using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Defaults;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Styles;

namespace Das.Views.Panels
{
    public class ContentPanel<T> : BaseContainerVisual<T>,
                                            IContentContainer,
                                            IVisualContainer,
                                            IContentPresenter
    {
        private readonly IVisualBootstrapper _visualBootstrapper;

        public ContentPanel(IVisualBootstrapper visualBootstrapper)
            : base(visualBootstrapper)
        {
            _contentMeasured = ValueSize.Empty;
            _visualBootstrapper = visualBootstrapper;
            _contentTemplate = new DefaultContentTemplate(visualBootstrapper, this);
        }

        public  ContentPanel(IVisualBootstrapper visualBootstrapper,
                             IDataBinding<T> binding)
            : base(visualBootstrapper, binding)
        {
            _contentMeasured = ValueSize.Empty;
            _visualBootstrapper = visualBootstrapper;
            _contentTemplate = new DefaultContentTemplate(visualBootstrapper, this);
        }


        protected TQuery QueryContent<TQuery>(Func<IVisualElement, TQuery> query,
                                              TQuery defaultValue)
        {
            if (!(Content is { } valid))
                return defaultValue;

            return query(valid);
        }

        public override Boolean IsChanged => IsRequiresMeasure || IsRequiresArrange || 
                                             Content is {} valid && 
                                             (valid.IsRequiresMeasure || valid.IsRequiresArrange);

        public override Boolean IsRequiresMeasure
        {
            get => base.IsRequiresMeasure || QueryContent(c => c.IsRequiresMeasure, false);
            protected set => base.IsRequiresMeasure = value;
        }

        public override Boolean IsRequiresArrange
        {
            get => base.IsRequiresArrange || QueryContent(c => c.IsRequiresArrange, false);
            protected set => base.IsRequiresArrange = value;
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
            var contentCanHave = GetMeasureSpace(measureContext, availableSpace,
                out var padding,
                out var mySize);

            _contentMeasured = Content is { } content
                ? measureContext.MeasureElement(content, contentCanHave)
                : ValueSize.Empty;

            var useHeight = Double.PositiveInfinity;

            if (Height.HasValue)
                useHeight = Height.Value;
            else if (VerticalAlignment == VerticalAlignments.Stretch)
                useHeight = mySize.Height;

            if (Double.IsInfinity(useHeight) || useHeight < _contentMeasured.Height)
            {
                useHeight = _contentMeasured.Height;
                if (padding != null)
                    useHeight += padding.Height;
            }

            //////////////////////////////////

            var useWidth = Double.PositiveInfinity;

            if (Width.HasValue)
                useWidth = Width.Value;
            else if (HorizontalAlignment == HorizontalAlignments.Stretch)
                useWidth = mySize.Width;

            if (Double.IsInfinity(useWidth) || useWidth < _contentMeasured.Width)
            {
                useWidth = _contentMeasured.Width;
                if (padding != null)
                    useWidth += padding.Width;
            }


            return new ValueSize(useWidth, useHeight);
        }

        public override void InvalidateMeasure()
        {
            if (Content is {} content)
                content.InvalidateMeasure();
            

            base.InvalidateMeasure();
            //IsChanged = true;
        }

        public override void InvalidateArrange()
        {
            if (Content is {} content)
                content.InvalidateArrange();
            

            base.InvalidateArrange();
            //IsChanged = true;
        }

        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
            if (Content is { } content)
            {
                var contentSpace = GetContentArrangeSpace(content, _contentMeasured,
                    renderContext, availableSpace, 
                    out _,
                    out _);

                if (contentSpace.Left < 0)
                {}

                renderContext.DrawElement(content, contentSpace);
            }

            IsRequiresArrange = false;
        }

        protected virtual IRenderSize GetMeasureSpace(IStyleProvider styleContext,
                                                     IRenderSize availableSpace,
                                                     out Thickness? padding,
                                                     out ValueSize mySize)
        {
            padding = GetPadding(styleContext);
            TryGetSize(out mySize);

            IRenderSize useAvailable;
            if (mySize.IsEmpty && padding?.IsEmpty != false)
                useAvailable = availableSpace;
            else if (mySize.IsEmpty)
                useAvailable = new ValueRenderSize(availableSpace, availableSpace.Offset, padding);
            else
                useAvailable = new ValueRenderSize(mySize, availableSpace.Offset, padding);

            if (mySize.IsEmpty)
                mySize = availableSpace.ToValueSize();

            return useAvailable;
        }

        protected virtual IRenderRectangle GetContentArrangeSpace(IVisualElement content,
                                                                  ISize contentMeasured,
            IStyleProvider styleContext,
                                                           IRenderSize availableSpace,
                                                           out Thickness? padding,
                                                           out ValueSize mySize)
        {
            padding = GetPadding(styleContext);
            TryGetSize(out mySize);


            Double left = 0, top = 0; 
            Double width, height;

            if (mySize.IsEmpty)
            {
                width = availableSpace.Width;
                height = availableSpace.Height;
            }
            else
            {
                width = mySize.Width;
                height = mySize.Height;
            }

            if (padding is {} goodPadding && !goodPadding.IsEmpty)
            {
                left += goodPadding.Left;
                top += goodPadding.Top;
                width -= padding.Width;
                height -= padding.Height;
            }

            var valign = content.VerticalAlignment;
            if (valign == VerticalAlignments.Default)
                valign = styleContext.GetStyleSetter<VerticalAlignments>(
                    StyleSetter.VerticalAlignment, content);

            var halign = content.HorizontalAlignment;
            if (halign == HorizontalAlignments.Default)
                halign = styleContext.GetStyleSetter<HorizontalAlignments>(
                    StyleSetter.HorizontalAlignment, content);

            //up to here we are giving content all available minus our padding
            var xDiff = width - contentMeasured.Width;
            var yDiff = height - contentMeasured.Height;


            switch (halign)
            {
                case HorizontalAlignments.Center:
                    
                    left += (xDiff / 2);
                    width = Math.Min(width, contentMeasured.Width);
                    break;

                case HorizontalAlignments.Right:
                    left += xDiff;
                    break;
            }

            switch (valign)
            {
                case VerticalAlignments.Center:
                    
                    top += (yDiff / 2);
                    height = Math.Min(height, contentMeasured.Height);
                    break;

                case VerticalAlignments.Bottom:
                    top += yDiff;
                    break;
            }

            return new RenderRectangle(left, top, width, height, availableSpace.Offset);

            //if (mySize.IsEmpty && padding?.IsEmpty != false)
            //    useAvailable = availableSpace.ToFullRectangle();
            //else if (mySize.IsEmpty)
            //    useAvailable = new ValueRenderRectangle(availableSpace, padding);
            //else
            //    useAvailable = new ValueRenderRectangle(mySize, availableSpace.Offset, padding);

            //return useAvailable;
        }

        protected virtual Thickness? GetPadding(IStyleProvider styleContext)
          {
            return styleContext.GetStyleSetter<Thickness>(StyleSetter.Padding, this);
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
            //IsChanged = false;
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

        protected override void OnDataContextChanged(Object? newValue)
        {
            base.OnDataContextChanged(newValue);

            switch (Content)
            {
                case IBindableElement bindable:
                    bindable.DataContext = newValue;
                    break;

                case {} _:
                    return;

                default:
                    //no content yet

                    Content = _contentTemplate.BuildVisual(newValue);

                    //if (!(newValue is { } valid))
                    //    return;
                    
                    //if (ContentTemplate is {} template)
                    //{
                    //    Content = template.BuildVisual(valid);
                    //    return;
                    //}

                    

                    //if (_visualBootstrapper.TryResolveFromContext(valid) is { } visual)
                    //    Content = visual;

                    break;
            }
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

            //IsChanged = true;
        }

        public override async Task SetDataContextAsync(Object? dataContext)
        {
            DataContext = dataContext;

            if (Content is IBindableElement bindable)
                await bindable.SetDataContextAsync(dataContext);

            //IsChanged = true;
        }


        public override void SetBoundValue(T value)
        {
            //IsChanged = true;
            Binding = new ObjectBinding<T>(value);

            if (Content is IBindableElement<T> bindable)
                bindable.SetBoundValue(value);
            else if (Content is IBindableElement almost)
                almost.SetDataContext(value);
        }

        public override async Task SetBoundValueAsync(T value)
        {
            //IsChanged = true;
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
            InvalidateMeasure();
            //IsChanged = true;
        }

        protected virtual Boolean OnContentChanging(IVisualElement? oldValue,
                                                    IVisualElement? newValue)
        {
            if (oldValue is { } old)
            {
                //if (old is INotifyPropertyChanged notifier)
                //    notifier.PropertyChanged -= OnChildPropertyChanged;
                old.OnParentChanging(null);
            }

            if (newValue is { } valid)
            {
                //if (valid is INotifyPropertyChanged notifier)
                //    notifier.PropertyChanged += OnChildPropertyChanged;
                valid.OnParentChanging(this);
            }

            return true;
        }

        public override void Dispose()
        {
            Content = null;
            base.Dispose();

            //if (Content is { } content)
            //    content.Dispose();
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
        private ValueSize _contentMeasured;

    }
}
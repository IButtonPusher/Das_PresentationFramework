using System;
using System.ComponentModel;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Defaults;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Styles;

namespace Das.Views.Panels
{
    public class ContentPanel : BaseContainerVisual,
                                IBindableContainer,
                                IContentVisual
    {
        public ContentPanel(IVisualBootstrapper visualBootstrapper) : base(visualBootstrapper)
        {
            _contentMeasured = ValueSize.Empty;
            _contentTemplate = new DefaultContentTemplate(visualBootstrapper);
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
            
        }

        public override void InvalidateArrange()
        {
            if (Content is {} content)
                content.InvalidateArrange();
            

            base.InvalidateArrange();
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

        protected virtual IRenderSize GetMeasureSpace(IVisualContext styleContext,
                                                      IRenderSize availableSpace,
                                                      out Thickness? padding,
                                                      out ValueSize mySize)
        {
            padding = GetPadding(styleContext);
            styleContext.TryGetElementSize(this, out mySize);

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
                                                                  IVisualContext visualContext,
                                                                  IRenderSize availableSpace,
                                                                  out Thickness? padding,
                                                                  out ValueSize mySize)
        {
            padding = GetPadding(visualContext);
            visualContext.TryGetElementSize(this, out mySize);


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

            if (padding is { } goodPadding && !goodPadding.IsEmpty)
            {
                left += goodPadding.Left;
                top += goodPadding.Top;
                width -= padding.Width;
                height -= padding.Height;
            }

            var valign = content.VerticalAlignment;
            if (valign == VerticalAlignments.Default)
                valign = visualContext.GetStyleSetter<VerticalAlignments>(
                    StyleSetterType.VerticalAlignment, content);

            var halign = content.HorizontalAlignment;
            if (halign == HorizontalAlignments.Default)
                halign = visualContext.GetStyleSetter<HorizontalAlignments>(
                    StyleSetterType.HorizontalAlignment, content);

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
        }

        protected virtual Thickness? GetPadding(IStyleProvider styleContext)
        {
            return styleContext.GetStyleSetter<Thickness>(StyleSetterType.Padding, this);
        }

        //public override IVisualElement DeepCopy()
        //{
        //    var newObject = (IContentContainer) base.DeepCopy();

        //    var content = Content;
        //    if (content == null)
        //        return newObject;

        //    var newContent = content.DeepCopy();
        //    newObject.Content = newContent;
        //    if (newObject is BindableElement<T> bindable)
        //        bindable.Binding = Binding;

        //    return newObject;
        //}


        public override void AcceptChanges()
        {
            base.AcceptChanges();

            //IsChanged = false;
            if (Content is IChangeTracking ct)
                ct.AcceptChanges();
        }

        public override void AcceptChanges(ChangeType changeType)
        {
            base.AcceptChanges(changeType);
            Content?.AcceptChanges(changeType);
        }


        public IDataTemplate? ContentTemplate
        {
            get => _contentTemplate;
            // ReSharper disable once UnusedMember.Global
            set => SetValue(ref _contentTemplate,
                value ?? _contentTemplate);
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
                    break;
            }
        }


        //public void OnChildDeserialized(IVisualElement element, INode node)
        //{
        //}

        //public virtual Boolean Contains(IVisualElement element)
        //{
        //    if (Content == element)
        //        return true;

        //    if (Content is IVisualContainer container)
        //        return container.Contains(element);

        //    return false;
        //}

        //public override void SetDataContext(Object? dataContext)
        //{
        //    DataContext = dataContext;

        //    if (Content is IBindableElement bindable)
        //        bindable.SetDataContext(dataContext);

        //    //IsChanged = true;
        //}

        //public override async Task SetDataContextAsync(Object? dataContext)
        //{
        //    DataContext = dataContext;

        //    if (Content is IBindableElement bindable)
        //        await bindable.SetDataContextAsync(dataContext);

        //    //IsChanged = true;
        //}


        //public override void SetBoundValue(T value)
        //{
        //    Binding = new ObjectBinding<T>(value);

        //    if (Content is IBindableElement<T> bindable)
        //        bindable.SetBoundValue(value);
        //    else if (Content is IBindableElement almost)
        //        almost.SetDataContext(value);
        //}

        //public override async Task SetBoundValueAsync(T value)
        //{
        //    Binding = new ObjectBinding<T>(value);

        //    if (Content is IBindableElement<T> bindable)
        //        await bindable.SetBoundValueAsync(value);
        //    else if (Content is IBindableElement almost)
        //        await almost.SetDataContextAsync(value);
        //}

        //protected IVisualElement? GetContent()
        //{
        //    if (Content is { } content)
        //        return content;

        //    if (!(Value is { } valid))
        //        return null;

        //    return Content = _contentTemplate.BuildVisual(valid);
        //}

        protected virtual void OnContentChanged(IVisualElement? obj)
        {
            InvalidateMeasure();
            
            if (obj is {} content)
                content.InvalidateMeasure();
        }

        protected virtual Boolean OnContentChanging(IVisualElement? oldValue,
                                                    IVisualElement? newValue)
        {
            if (oldValue is { } old)
            {
                old.OnParentChanging(null);
            }

            if (newValue is { } valid)
            {
                valid.OnParentChanging(this);
            }

            return true;
        }

        public override void Dispose()
        {
            Content = null;
            base.Dispose();
        }

        public void UpdateContentDataContext(Object? newValue)
        {
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
                    break;
            }
        }


        private IVisualElement? _content;
        private IDataTemplate _contentTemplate;
        private ValueSize _contentMeasured;

        //public ContentPanel(IVisualBootstrapper visualBootstrapper, 
        //                    IDataBinding<Object> binding) 
        //    : base(visualBootstrapper, binding)
        //{
        //}
    }
}


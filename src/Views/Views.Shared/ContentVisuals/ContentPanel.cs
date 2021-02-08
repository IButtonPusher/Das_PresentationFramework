using System;
using System.ComponentModel;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.DataBinding;
using Das.Views.Defaults;
using Das.Views.Primitives;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Styles.Declarations;

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

        public static readonly DependencyProperty<IContentVisual, QuantifiedThickness> PaddingProperty =
            DependencyProperty<IContentVisual, QuantifiedThickness>.Register(
                nameof(Padding), QuantifiedThickness.Empty);

        public QuantifiedThickness Padding
        {
            get => PaddingProperty.GetValue(this);
            set => PaddingProperty.SetValue(this, value);
        }

        public override ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
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
                //if (padding != null)
                if (!padding.IsEmpty)
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
                
                if (!padding.IsEmpty)
                    useWidth += padding.Width;
            }


            return new ValueSize(useWidth, useHeight);
        }

        public String FontName
        {
            get => TextBase.FontNameProperty.GetValue(this);
            set => TextBase.FontNameProperty.SetValue(this, value);
        }

        public Double FontSize
        {
            get => TextBase.FontSizeProperty.GetValue(this);
            set => TextBase.FontSizeProperty.SetValue(this, value);
        }

        public FontStyle FontWeight
        {
            get => TextBase.FontWeightProperty.GetValue(this);
            set => TextBase.FontWeightProperty.SetValue(this, value);
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

        public override void Arrange<TRenderSize>(TRenderSize availableSpace,
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
                                                      out ValueThickness padding,
                                                      out ValueSize mySize)
        {
            padding = Padding.GetValue(availableSpace);
            
            styleContext.TryGetElementSize(this, availableSpace, out mySize);

            IRenderSize useAvailable;
            if (mySize.IsEmpty && padding.IsEmpty)
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
                                                                  out ValueThickness padding,
                                                                  out ValueSize mySize)
        {
            padding = Padding.GetValue(availableSpace);
            
            visualContext.TryGetElementSize(this, availableSpace, out mySize);


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
           
            var halign = content.HorizontalAlignment;
            
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

        public override void AcceptChanges()
        {
            base.AcceptChanges();

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

                    var justAdded = _contentTemplate.BuildVisual(newValue);
                    if (justAdded is IFontVisual fonty)
                    {
                        fonty.FontSize = FontSize;
                        fonty.FontName = FontName;
                        fonty.FontWeight = FontWeight;
                    }
                    Content = justAdded;
                    break;
            }
        }


       
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

        public override Boolean TryGetDependencyProperty(DeclarationProperty declarationProperty,
                                                         out IDependencyProperty property)
        {
            switch (declarationProperty)
            {
                case DeclarationProperty.Padding:
                    property = PaddingProperty;
                    return true;
            }

            return base.TryGetDependencyProperty(declarationProperty, out property);
        }


        private IVisualElement? _content;
        private IDataTemplate _contentTemplate;
        private ValueSize _contentMeasured;
    }
}


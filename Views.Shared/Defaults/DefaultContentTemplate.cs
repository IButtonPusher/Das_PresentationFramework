using System;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.DataBinding;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Defaults
{
    public class DefaultContentTemplate : BindableElement,
                                          IDataTemplate
                                          
    {
        public DefaultContentTemplate(IVisualBootStrapper templateResolver,
                                      IVisualElement? host) : base(templateResolver)
        {
            _templateResolver = templateResolver;
            _bindable = host as IBindableElement;
        }

        public Type? DataType => null;

        public virtual IVisualElement BuildVisual(Object? dataContext) => throw new NotSupportedException();

        IVisualElement IDataTemplate.Template => GetTemplate() ?? this;

        public override  ValueSize Measure(IRenderSize availableSpace, 
                                           IMeasureContext measureContext)
        {
            var visual = GetTemplate();

            if (visual is { } validVisual)
                return measureContext.MeasureElement(validVisual, availableSpace);

            if (GetString() is {} validValue)
            {
                var font = measureContext.GetStyleSetter<IFont>(
                    StyleSetter.Font, this);
                return measureContext.MeasureString(validValue, font);
            }

            return ValueSize.Empty;
        }

        private String? GetString()
        {
            if (BoundValue != null)
                return BoundValue.ToString();

            if (_bindable?.DataContext is { } ok)
                return ok.ToString();

            return null;
        }

        public override Object? GetBoundValue(Object dataContext)
        {
            return base.GetBoundValue(dataContext) ?? _bindable?.DataContext;
        }

        private IVisualElement? GetTemplate()
        {
            if (_resolvedTemplate is { } valid)
                return valid;

            if (_hasTriedResolvingTemplate)
                return _resolvedTemplate;

            var bound = BoundValue ?? _bindable?.DataContext;


            //if (_hasTriedResolvingTemplate || 
            //    !(BoundValue is { } value))
            //    return null;


            _hasTriedResolvingTemplate = true;

            if (bound == null)
                return null;

            return _resolvedTemplate = _templateResolver.TryResolve(bound);

        }

        public override void Arrange(IRenderSize availableSpace, 
                                     IRenderContext renderContext)
        {
            var visual = GetTemplate();

            if (visual is { } validVisual)
                renderContext.DrawElement(validVisual, availableSpace.ToFullRectangle());

            else if (GetString() is {} validValue)
            {
                var color = renderContext.GetStyleSetter<SolidColorBrush>(
                    StyleSetter.Foreground, this);
                var font = renderContext.GetStyleSetter<IFont>(
                    StyleSetter.Font, this);
                renderContext.DrawString(validValue.ToString(), font, color,
                    Point2D.Empty);
            }
        }


        private IVisualElement? _resolvedTemplate;
        private Boolean _hasTriedResolvingTemplate;

        private readonly IVisualBootStrapper _templateResolver;
        private readonly IBindableElement? _bindable;
    }
}

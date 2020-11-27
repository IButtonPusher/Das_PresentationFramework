using System;
using Das.Extensions;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
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
        public DefaultContentTemplate(IVisualBootStrapper visualBootStrapper,
                                      IVisualElement? host) : base(visualBootStrapper)
        {
            _visualBootStrapper = visualBootStrapper;
            _bindable = host as IBindableElement;
            _contentMeasured = ValueSize.Empty;
        }

        public Type? DataType => null;

        public virtual IVisualElement? BuildVisual(Object? dataContext)
        {
            if (dataContext == null)
                return default;

            var txt = new Label<Object>(
                new ObjectBinding<Object>(dataContext),
                _visualBootStrapper);
            txt.HorizontalAlignment = HorizontalAlignments.Center;
            txt.VerticalAlignment = VerticalAlignments.Center;

            return txt;

        }

        //IVisualElement IDataTemplate.Template => GetTemplate() ?? this;

        public override  ValueSize Measure(IRenderSize availableSpace, 
                                           IMeasureContext measureContext)
        {
            var visual = GetTemplate();

            if (visual is { } validVisual)
                _contentMeasured = measureContext.MeasureElement(validVisual, availableSpace);

            else if (GetString() is {} validValue)
            {
                var forStyle = GetStylableElement();

                var font = measureContext.GetStyleSetter<IFont>(
                    StyleSetter.Font, forStyle);
                var fontSize = measureContext.GetStyleSetter<Double>(
                    StyleSetter.FontSize, forStyle);
                if (!Double.IsNaN(fontSize) && fontSize.AreDifferent(font.Size))
                    font = font.Resize(fontSize);

                _contentMeasured = measureContext.MeasureString(validValue, font);
            }
            else _contentMeasured = ValueSize.Empty;

            return _contentMeasured;
        }

        private IVisualElement GetStylableElement()
        {
            return _bindable ?? this;
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

        protected virtual IVisualElement? GetTemplate()
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

            return _resolvedTemplate = _visualBootStrapper.TryResolveFromContext(bound);

        }

        public override void Arrange(IRenderSize availableSpace, 
                                     IRenderContext renderContext)
        {
            var visual = GetTemplate();

            if (visual is { } validVisual)
                renderContext.DrawElement(validVisual, availableSpace.ToFullRectangle());

            else if (GetString() is {} validValue)
            {
                var forStyle = GetStylableElement();

                var color = renderContext.GetStyleSetter<SolidColorBrush>(
                    StyleSetter.Foreground, this);
                var font = renderContext.GetStyleSetter<IFont>(
                    StyleSetter.Font, forStyle);
                var fontSize = renderContext.GetStyleSetter<Double>(
                    StyleSetter.FontSize, forStyle);
                if (!Double.IsNaN(fontSize) && fontSize.AreDifferent(font.Size))
                    font = font.Resize(fontSize);

                var x = availableSpace.CenterX(_contentMeasured);
                var y = availableSpace.CenterY(_contentMeasured);
                

                renderContext.DrawString(validValue, font, color,
                    new ValuePoint2D(x,y));
            }
        }


        private IVisualElement? _resolvedTemplate;
        private Boolean _hasTriedResolvingTemplate;

        private readonly IVisualBootStrapper _visualBootStrapper;
        private readonly IBindableElement? _bindable;
        private ValueSize _contentMeasured;
    }
}

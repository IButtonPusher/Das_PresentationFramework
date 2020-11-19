using Das.Views.Rendering;
using System;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Styles;

namespace Das.Views
{
    public class HorizontalRule : VisualElement

    {
        public override ValueSize Measure(IRenderSize availableSpace, 
                                          IMeasureContext measureContext)
        {
            if (!(measureContext.ViewState is { } viewState))
                return new ValueSize(availableSpace.Width, 1);

            var zoom = viewState.ZoomLevel;

            var specificHeight = viewState.GetStyleSetter<Double>(StyleSetter.Height, this)
                                 * zoom;

            specificHeight = Double.IsNaN(specificHeight) ? availableSpace.Height : specificHeight;

            var specificWidth = viewState.GetStyleSetter<Double>(StyleSetter.Width, this)
                                * zoom;
            specificWidth = Double.IsNaN(specificWidth) ? availableSpace.Width : specificWidth;

            return new ValueSize(specificWidth, specificHeight);
        }

        public override void Arrange(IRenderSize availableSpace, 
                                     IRenderContext renderContext)
        {
            var bg = renderContext.GetStyleSetter<SolidColorBrush>(StyleSetter.Background, this);

            renderContext.FillRectangle(availableSpace.ToFullRectangle(), bg);
        }

        public HorizontalRule(IVisualBootStrapper templateResolver) : base(templateResolver)
        {
        }
    }
}

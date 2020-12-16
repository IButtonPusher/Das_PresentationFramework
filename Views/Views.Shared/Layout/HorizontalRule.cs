using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views
{
    public sealed class HorizontalRule : VisualElement

    {
        public HorizontalRule(IVisualBootstrapper visualBootstrapper) 
            : base(visualBootstrapper)
        {
           InvalidateMeasure();
        }

        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
            //Debug.WriteLine("arrange hrule");

            var bg = renderContext.GetStyleSetter<SolidColorBrush>(StyleSetter.Background, this);

            renderContext.FillRectangle(availableSpace.ToFullRectangle(), bg);
        }

        public override ValueSize Measure(IRenderSize availableSpace,
                                          IMeasureContext measureContext)
        {
            //Debug.WriteLine("measure hrule");

            if (!(measureContext.ViewState is { } viewState))
                return new ValueSize(availableSpace.Width, 1);

            var specificHeight = viewState.GetStyleSetter<Double>(StyleSetter.Height, this);

            specificHeight = Double.IsNaN(specificHeight) ? availableSpace.Height : specificHeight;

            var specificWidth = viewState.GetStyleSetter<Double>(StyleSetter.Width, this);
                                
            specificWidth = Double.IsNaN(specificWidth) ? availableSpace.Width : specificWidth;

            return new ValueSize(specificWidth, specificHeight);
        }
    }
}
using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Views.Controls
{
    public class HtmlPanel : BaseSurrogatedVisual
    {

        private String? _markup;

        public String? Markup
        {
            get => _markup;
            set => SetValue(ref _markup, value);
        }

        public override void Arrange(ISize availableSpace,
                                     IRenderContext renderContext)
        {
            throw new NotSupportedException("A surrogate control is required for this control");
        }

        public override ISize Measure(ISize availableSpace,
                                      IMeasureContext measureContext)
        {
            throw new NotSupportedException("A surrogate control is required for this control");
        }
    }
}
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Views.Controls
{
    public class HtmlPanel : BaseSurrogatedVisual
    {
        public HtmlPanel(IVisualBootstrapper visualBootstrapper) 
            : base(visualBootstrapper)
        {
        }

        public String? Markup
        {
            get => _markup;
            set => SetValue(ref _markup, value, OnMarkupChanged);
        }

        public Uri? Uri
        {
            get => _uri;
            set => SetValue(ref _uri, value);
        }

        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
            throw new NotSupportedException("A surrogate control is required for this control");
        }

        public override ValueSize Measure(IRenderSize availableSpace,
                                          IMeasureContext measureContext)
        {
            throw new NotSupportedException("A surrogate control is required for this control");
        }

        private void OnMarkupChanged(String? newVal)
        {
            Debug.WriteLine("changing markup in html surrogater:\r\n" + newVal);
        }

        private String? _markup;

        private Uri? _uri;
    }
}
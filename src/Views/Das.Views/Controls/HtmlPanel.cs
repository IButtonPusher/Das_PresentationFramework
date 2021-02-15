using System;
using System.Threading.Tasks;
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
            set => SetValue(ref _markup, value);
        }

        public Uri? Uri
        {
            get => _uri;
            private set => SetValue(ref _uri, value);
        }

        public void SetUri(Uri? value)
        {
            if (value == null)
            {
                Uri = value;
                return;
            }

            if (value.Scheme == "http")
            {
                //ensure https
                value = new Uri("https://" + value.Authority + value.AbsolutePath);
            }

            Uri = value;
        }

        private String? _markup;

        private Uri? _uri;
    }
}
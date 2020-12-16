using System;

namespace Das.Views.Styles
{
    public class DefaultStyleContext : BaseStyleContext
    {
        public DefaultStyleContext() : base(DefaultStyle.Instance,
            new DefaultColorPalette())
        {
        }
    }
}

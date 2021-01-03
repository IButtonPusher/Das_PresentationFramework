using System;
using Das.Views.Construction.Styles;

namespace Das.Views.Styles
{
    public class DefaultStyleContext : BaseStyleContext
    {
        public static readonly DefaultStyleContext Instance = new DefaultStyleContext();
        
        
        private DefaultStyleContext() : base(DefaultStyle.Instance,
            Styles.ColorPalette.Baseline, new StyleVariableAccessor())
        {
        }
    }
}

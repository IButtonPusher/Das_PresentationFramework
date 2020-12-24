using System;

namespace Das.Views.Construction
{
    public class CssMarkupNode : MarkupNode
    {
        public CssMarkupNode(String selectorText, 
                             MarkupNode? parent) 
            : base(selectorText, parent, false, MarkupLanguage.Css)
        {
        }
    }
}

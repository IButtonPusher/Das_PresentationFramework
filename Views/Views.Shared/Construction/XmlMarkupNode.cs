using System;

namespace Das.Views.Construction
{
    public class XmlMarkupNode : MarkupNode
    {
        public XmlMarkupNode(String name, 
                             MarkupNode? parent) 
            : base(name, parent, 
                String.Equals(name, "?xml", StringComparison.OrdinalIgnoreCase),
                MarkupLanguage.Xml)
        {
        }
    }
}

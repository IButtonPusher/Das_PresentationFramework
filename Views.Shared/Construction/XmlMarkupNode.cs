using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Construction
{
    public class XmlMarkupNode : MarkupNode
    {
        public XmlMarkupNode(String name, 
                             MarkupNode? parent) 
            : base(name, parent, 
                String.Equals(name, "?xml", StringComparison.OrdinalIgnoreCase))
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Construction
{
    public interface IMarkupNode
    {
        Boolean IsEncodingHeader { get; }

        IMarkupNode this[Int32 index] { get; }

        Int32 ChildrenCount { get; }

        String Name { get; }

        IEnumerable<IMarkupNode> Children { get; }

        Boolean TryGetAttributeValue(String key, 
                                     out String value);

        IEnumerable<KeyValuePair<String, String>> GetAllAttributes();
    }
}

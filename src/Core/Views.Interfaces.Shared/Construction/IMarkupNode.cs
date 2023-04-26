using System;
using System.Collections.Generic;

namespace Das.Views.Construction;

public interface IMarkupNode : IAttributeDictionary
{
   Boolean IsEncodingHeader { get; }

   IMarkupNode this[Int32 index] { get; }

   Int32 ChildrenCount { get; }

   String Name { get; }
        
   String? InnerText { get; }
        
   MarkupLanguage Language { get; }

   IEnumerable<IMarkupNode> Children { get; }
}
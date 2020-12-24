using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Das.Views.Construction
{
    public class ChildObjectResult
    {
        public ChildObjectResult(Object? child, 
                                 ChildNodeType childType, 
                                 PropertyInfo? visualProperty)
        {
            Child = child;
            ChildType = childType;
            VisualProperty = visualProperty;
        }

        public Object? Child { get; }
        
        public ChildNodeType ChildType { get; }

        public PropertyInfo? VisualProperty { get; }
    }
}

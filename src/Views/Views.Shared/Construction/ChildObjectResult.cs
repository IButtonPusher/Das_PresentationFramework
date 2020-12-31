using System;
using Das.Serializer;

namespace Das.Views.Construction
{
    public readonly struct ChildObjectResult
    {
        public ChildObjectResult(Object? child, 
                                 ChildNodeType childType, 
                                 IPropertyAccessor? visualProperty)
                                // PropertyInfo? visualProperty)
        {
            Child = child;
            ChildType = childType;
            VisualProperty = visualProperty;
        }

        public Object? Child { get; }
        
        public ChildNodeType ChildType { get; }

        public IPropertyAccessor? VisualProperty { get; }
        
        //public PropertyInfo? VisualProperty { get; }
    }
}

using System;

namespace Das.Views.Styles
{
    public readonly struct CascadingAssignedStyle : IEquatable<CascadingAssignedStyle>,
                                                    IStyleSetter
    {
        public CascadingAssignedStyle(Type? targetType,
                                      IStyle style)
        {
            TargetType = targetType;
            Style = style;
        }

        public Type? TargetType { get; }
        
        public IStyle Style { get; }
        
        //public AssignedStyle AssignedStyle { get; }

        public Boolean Equals(CascadingAssignedStyle other)
        {
            return other.TargetType == TargetType;
        }

        Boolean IEquatable<IStyleSetter>.Equals(IStyleSetter other)
        {
            return other is CascadingAssignedStyle casc && Equals(casc);
        }

        Object? IStyleSetter.Value => Style;
    }
}

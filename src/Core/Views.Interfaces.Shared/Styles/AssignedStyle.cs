using System;
using System.Threading.Tasks;

namespace Das.Views.Styles;

public readonly struct AssignedStyle : IEquatable<AssignedStyle>,
                                       IStyleSetter
{
   public AssignedStyle(StyleSetterType setterType,
                        VisualStateType type,
                        Object? value = null)
   {
      SetterType = setterType;
      Type = type;
      Value = value;

      _hash = (Int32) setterType + ((Int32) type << 16);
   }

   public readonly StyleSetterType SetterType;
   public readonly VisualStateType Type;
   public Object? Value { get; }

   private readonly Int32 _hash;

   public Boolean Equals(AssignedStyle other)
   {
      return other.SetterType == SetterType && other.Type == Type;
   }

   public Boolean Equals(IStyleSetter other)
   {
      return other is AssignedStyle assigned && Equals(assigned);
   }

   public override Boolean Equals(Object obj)
   {
      return obj is AssignedStyle assigned && Equals(assigned);
   }

   public override Int32 GetHashCode()
   {
      return _hash;
   }

   public override String ToString()
   {
      if (Type == VisualStateType.None)
         return SetterType + " = " + Value;

      return SetterType + ":" + Type + " = " + Value;
   }
}
using System;
using System.Threading.Tasks;

namespace Das.Views.DependencyProperties;

public struct LocalValueEntry
{
   public IDependencyProperty _dp;
   public Object? _value;

   public override Int32 GetHashCode()
   {
      // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
      return base.GetHashCode();
   }

   public override Boolean Equals(Object obj)
   {
      var localValueEntry = (LocalValueEntry)obj;
      return _dp == localValueEntry._dp && _value == localValueEntry._value;
   }

   public static Boolean operator ==(LocalValueEntry obj1,
                                     LocalValueEntry obj2)
   {
      return obj1.Equals(obj2);
   }

   public static Boolean operator !=(LocalValueEntry obj1,
                                     LocalValueEntry obj2)
   {
      return !(obj1 == obj2);
   }

   public IDependencyProperty Property => _dp;

   public Object? Value => _value;

   public LocalValueEntry(IDependencyProperty dp,
                          Object? value)
   {
      _dp = dp;
      _value = value;
   }
}
using System;
using System.Collections.Generic;

namespace Das.Views.Styles.Declarations.Transition;

public class TransitionDurationDeclaration : QuantifiedNumericDeclaration<TimeUnit>
{
   public TransitionDurationDeclaration(String value/*, 
                                        IStyleVariableAccessor variableAccessor*/) 
      : base(value, _defaultQuantitySearch,
         TimeUnit.S, //variableAccessor,
         DeclarationProperty.TransitionDuration)
   {
   }

   public TimeSpan ToTimeSpan()
   {
      switch (Units)
      {
         case TimeUnit.Ms:
            return TimeSpan.FromMilliseconds(Value);

         case TimeUnit.S:
            return TimeSpan.FromSeconds(Value);

         default:
            throw new NotImplementedException();
      }
   }

   public override String ToString()
   {
      return Value + "" + Units;
   }

   public static Boolean IsValidQuantity(String value)
   {
      return IsValidQuantity(value, TimeUnit.Invalid, _defaultQuantitySearch);
   }
        
   private static readonly Dictionary<String, TimeUnit> _defaultQuantitySearch = 
      new Dictionary<String, TimeUnit>();
}
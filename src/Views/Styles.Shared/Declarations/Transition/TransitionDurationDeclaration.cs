using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Styles.Declarations.Transition
{
    public class TransitionDurationDeclaration : QuantifiedNumericDeclaration<TimeUnit>
    {
        public TransitionDurationDeclaration(String value, 
                                             IStyleVariableAccessor variableAccessor) 
            : base(value, _defaultQuantitySearch,
                TimeUnit.S, variableAccessor, DeclarationProperty.TransitionDuration)
        {
        }

        public static Boolean IsValidQuantity(String value)
        {
            return IsValidQuantity(value, TimeUnit.Invalid, _defaultQuantitySearch);
        }
        
        private static readonly Dictionary<String, TimeUnit> _defaultQuantitySearch = 
            new Dictionary<String, TimeUnit>();
    }
}

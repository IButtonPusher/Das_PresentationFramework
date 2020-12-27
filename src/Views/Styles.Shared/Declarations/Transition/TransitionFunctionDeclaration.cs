using System;

namespace Das.Views.Styles.Declarations.Transition
{
    public class TransitionFunctionDeclaration : EnumDeclaration<TransitionFunctionType>
    {
        // ReSharper disable once UnusedMember.Global
        public TransitionFunctionDeclaration(String value, 
                                             TransitionFunctionType defaultValue, 
                                             IStyleVariableAccessor variableAccessor) 
            : base(value, defaultValue, variableAccessor, DeclarationProperty.TransitionTimingFunction)
        {
        }
        
        public TransitionFunctionDeclaration(TransitionFunctionType value, 
                                             IStyleVariableAccessor variableAccessor) 
            : base(value, variableAccessor, DeclarationProperty.TransitionTimingFunction)
        {
        }
    }
}

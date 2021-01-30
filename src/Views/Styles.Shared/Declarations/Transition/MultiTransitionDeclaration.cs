using System;
using System.Collections.Generic;

namespace Das.Views.Styles.Declarations.Transition
{
    public class MultiTransitionDeclaration : MultiValueDeclaration<TransitionDeclaration>
    {
        public MultiTransitionDeclaration(String value, 
                                          IStyleVariableAccessor variableAccessor) 
            : base(
                GetTransitions(value, variableAccessor),
                variableAccessor, DeclarationProperty.Transition)
        {
        }

        private static IEnumerable<TransitionDeclaration> GetTransitions(String value,
                                                                         IStyleVariableAccessor variableAccessor)
        {
            var tokens = GetMultiSplit(value, ',');
            foreach (var token in tokens)
                yield return new TransitionDeclaration(token, variableAccessor);
        }
    }
}

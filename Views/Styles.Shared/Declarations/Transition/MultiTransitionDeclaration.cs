using System;
using System.Collections.Generic;

namespace Das.Views.Styles.Declarations.Transition
{
    public class MultiTransitionDeclaration : DeclarationBase
    {
        public MultiTransitionDeclaration(String value, 
                                          IStyleVariableAccessor variableAccessor) 
            : base(variableAccessor, DeclarationProperty.Transition)
        {
            var tokens = value.Split(',');
            _transitions = new List<TransitionDeclaration>();
            
            for (var c = 0; c < tokens.Length; c++)
            {
                var token = tokens[c].Trim();

                var transition = new TransitionDeclaration(token, variableAccessor);
                _transitions.Add(transition);
            }
            
        }

        private readonly List<TransitionDeclaration> _transitions;

        public IEnumerable<TransitionDeclaration> Transitions => _transitions;
    }
}

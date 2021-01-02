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
            //var tokens = value.Split(',');
            //_transitions = new List<TransitionDeclaration>();
            
            //for (var c = 0; c < tokens.Length; c++)
            //{
            //    var token = tokens[c].Trim();

            //    var transition = new TransitionDeclaration(token, variableAccessor);
            //    _transitions.Add(transition);
            //}
        }

        private static IEnumerable<TransitionDeclaration> GetTransitions(String value,
                                                                         IStyleVariableAccessor variableAccessor)
        {
            var tokens = GetMultiSplit(value, ',');
            foreach (var token in tokens)
                yield return new TransitionDeclaration(token, variableAccessor);
        }

        //private readonly List<TransitionDeclaration> _transitions;

        //public IEnumerable<TransitionDeclaration> Transitions => _transitions;
    }
}

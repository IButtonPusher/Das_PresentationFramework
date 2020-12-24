using System;

namespace Das.Views.Styles.Declarations.Transition
{
    public class TransitionDeclaration : DeclarationBase
    {
        public TransitionDeclaration(String value, 
                                     IStyleVariableAccessor variableAccessor) 
            : base(variableAccessor, DeclarationProperty.Transition)
        {
            var tokens = value.Split();
            TransitionProperty = new TransitionPropertyDeclaration(tokens[0],
                DeclarationProperty.Invalid, variableAccessor);

            if (tokens.Length == 1)
            {
                Duration = new TransitionDurationDeclaration("0", variableAccessor);
                goto finished;
            }
            
            Duration = new TransitionDurationDeclaration(tokens[1], variableAccessor);

            if (tokens.Length == 2)
                goto finished;

            for (var c = 2; c < tokens.Length; c++)
            {
                var fn = GetEnumValue(tokens[c], TransitionFunctionType.Invalid, false);
                if (fn != TransitionFunctionType.Invalid)
                    TimingFunction = new TransitionFunctionDeclaration(fn, variableAccessor);
                else
                {
                    if (TransitionDurationDeclaration.IsValidQuantity(tokens[c]))
                        Delay = new TransitionDurationDeclaration(tokens[c], variableAccessor);
                }
            }


            finished:
            if (TimingFunction == null)
                TimingFunction = new TransitionFunctionDeclaration(TransitionFunctionType.Initial,
                    variableAccessor);

        }
        
        public TransitionPropertyDeclaration TransitionProperty { get; }
        
        public TransitionDurationDeclaration Duration { get; }
        
        public TransitionFunctionDeclaration? TimingFunction { get; }
        
        public TransitionDurationDeclaration? Delay { get; }
        
        
        
    }
}

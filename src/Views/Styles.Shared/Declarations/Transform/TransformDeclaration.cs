using System;
using Das.Views.Styles.Functions;

namespace Das.Views.Styles.Declarations.Transform
{
    public class TransformDeclaration : DeclarationBase
    {
        public TransformDeclaration(String value,
            IStyleVariableAccessor variableAccessor) 
            : base(variableAccessor, DeclarationProperty.Transform)
        {
            Function = FunctionBuilder.GetFunction(value, variableAccessor);
        }

        public IFunction Function { get; }
    }
}

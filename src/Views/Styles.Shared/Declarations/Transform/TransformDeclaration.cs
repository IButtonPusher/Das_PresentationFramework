using System;
using Das.Views.Styles.Functions;

namespace Das.Views.Styles.Declarations.Transform;

public class TransformDeclaration : ValueDeclaration<IFunction>
{
   public TransformDeclaration(String value,
                               IStyleVariableAccessor variableAccessor) 
      : base(FunctionBuilder.GetFunction(value, variableAccessor),
         /*variableAccessor, */DeclarationProperty.Transform)
   {
      Function = Value as ParameterizedFunction ?? throw new InvalidOperationException();

      TransformType = GetEnumValue(Function.FunctionName, TransformType.Invalid);
   }

   public ParameterizedFunction Function { get; }

   public TransformType TransformType {get;}
}
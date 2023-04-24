using System;

namespace Das.Views.Styles.Declarations;

public class OutlineWidthDeclaration : QuantityDeclaration
{
   public OutlineWidthDeclaration(String value/*, 
                                  IStyleVariableAccessor variableAccessor*/) 
      : base(value, /*variableAccessor, */DeclarationProperty.OutlineWidth)
   {
   }
}
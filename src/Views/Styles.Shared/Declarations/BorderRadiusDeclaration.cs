using System;

namespace Das.Views.Styles.Declarations;

public class BorderRadiusDeclaration : QuadQuantityDeclaration
{
   public BorderRadiusDeclaration(String value/*,
                                  IStyleVariableAccessor variableAccessor*/)
      : base(value, /*variableAccessor, */DeclarationProperty.BorderRadius)
   {
   }
}
using System;

namespace Das.Views.Styles.Declarations;

public class MarginDeclaration : QuadQuantityDeclaration
{
   public MarginDeclaration(String value/*,
                                 IStyleVariableAccessor variableAccessor*/)
      : base(value, /*variableAccessor, */DeclarationProperty.Margin)
   //DeclarationProperty.MarginTop,
   //DeclarationProperty.MarginRight,
   //DeclarationProperty.MarginBottom,
   //DeclarationProperty.MarginLeft)
   {
   }
}
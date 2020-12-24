using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Styles.Declarations
{
    public class BorderRadiusDeclaration : QuadQuantityDeclaration
    {
        public BorderRadiusDeclaration(String value,
                                       IStyleVariableAccessor variableAccessor)
            : base(value, variableAccessor, DeclarationProperty.BorderRadius,
                DeclarationProperty.BorderRadiusTop,
                DeclarationProperty.BorderRadiusRight,
                DeclarationProperty.BorderRadiusBottom,
                DeclarationProperty.BorderRadiusLeft)
        {
        }
    }
}
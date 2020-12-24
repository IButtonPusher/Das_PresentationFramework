using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Styles.Declarations
{
    public class MarginDeclaration : QuadQuantityDeclaration
    {
        public MarginDeclaration(String value,
                                 IStyleVariableAccessor variableAccessor)
            : base(value, variableAccessor, DeclarationProperty.Margin,
                DeclarationProperty.MarginTop,
                DeclarationProperty.MarginRight,
                DeclarationProperty.MarginBottom,
                DeclarationProperty.MarginLeft)
        {
        }
    }
}

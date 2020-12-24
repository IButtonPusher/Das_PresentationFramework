using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Styles.Declarations.Outline
{
    public class OutlineWidthDeclaration : QuantityDeclaration
    {
        public OutlineWidthDeclaration(String value, 
                                       IStyleVariableAccessor variableAccessor) 
            : base(value, variableAccessor, DeclarationProperty.OutlineWidth)
        {
        }
    }
}

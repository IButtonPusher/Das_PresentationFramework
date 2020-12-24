using System;

namespace Das.Views.Styles.Declarations
{
    public class LineHeightDeclaration : QuantityDeclaration
    {
        public LineHeightDeclaration(String value,
                                     IStyleVariableAccessor variableAccessor) 
            : base(value, variableAccessor, DeclarationProperty.LineHeight)
        {
        }
    }
}

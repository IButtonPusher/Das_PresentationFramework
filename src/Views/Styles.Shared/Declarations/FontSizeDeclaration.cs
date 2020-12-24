using System;

namespace Das.Views.Styles.Declarations
{
    public class FontSizeDeclaration : QuantityDeclaration
    {
        public FontSizeDeclaration(String value,
                                   IStyleVariableAccessor variableAccessor) 
            : base(value, variableAccessor, DeclarationProperty.FontSize)
        {
            
        }
    }
}

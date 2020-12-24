using System;

namespace Das.Views.Styles.Declarations.Outline
{
    public class OutlineColorDeclaration : ColorDeclaration
    {
        public OutlineColorDeclaration(String value, 
                                       IStyleVariableAccessor variableAccessor) 
            : base(value, DeclarationProperty.OutlineColor, variableAccessor)
        {
        }
    }
}

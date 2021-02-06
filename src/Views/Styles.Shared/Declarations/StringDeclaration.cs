using System;

namespace Das.Views.Styles.Declarations
{
    public class StringDeclaration : ValueDeclaration<String>
    {
        public StringDeclaration(String value,
                                 IStyleVariableAccessor variableAccessor, 
                                 DeclarationProperty property) 
            : base(value, variableAccessor, property)
        {
         
        }
    }
}

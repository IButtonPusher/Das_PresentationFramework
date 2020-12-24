using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Styles.Declarations
{
    public class StringDeclaration : DeclarationBase
    {
        public String Value { get; }

        public StringDeclaration(String value,
                                 IStyleVariableAccessor variableAccessor, 
                                 DeclarationProperty property) 
            : base(variableAccessor, property)
        {
            Value = value;
        }

        public override String ToString()
        {
            return Property + ": " + Value;
        }
    }
}

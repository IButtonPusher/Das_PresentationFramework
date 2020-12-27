using System;

namespace Das.Views.Styles.Declarations
{
    public class StringDeclaration : ValueDeclaration<String>
    {
        //public String Value { get; }

        public StringDeclaration(String value,
                                 IStyleVariableAccessor variableAccessor, 
                                 DeclarationProperty property) 
            : base(value, variableAccessor, property)
        {
          //  Value = value;
        }

        //public override String ToString()
        //{
        //    return Property + ": " + Value;
        //}
    }
}

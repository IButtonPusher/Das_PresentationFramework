using System;

namespace Das.Views.Styles.Declarations
{
    public class EnumDeclaration<TEnum> : DeclarationBase
        where TEnum : struct
    {
        public EnumDeclaration(String value,
                               TEnum defaultValue,
                               IStyleVariableAccessor variableAccessor,
                               DeclarationProperty property)
            : base(variableAccessor, property)
        {
            Value = GetEnumValue(value, defaultValue);
        }

        public EnumDeclaration(TEnum value,
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

        public TEnum Value { get; }
        
      

    }
}

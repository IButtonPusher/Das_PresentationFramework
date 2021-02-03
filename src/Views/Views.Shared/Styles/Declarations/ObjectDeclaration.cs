using System;

namespace Das.Views.Styles.Declarations
{
    public class ObjectDeclaration : IStyleDeclaration
    {
        public Object? Value { get; }

        public ObjectDeclaration(Object? value)
        {
            Value = value;
        }
        
        public DeclarationProperty Property => DeclarationProperty.Invalid;

        public Boolean Equals(IStyleDeclaration other)
        {
            return other.Property == Property &&
                   other is ObjectDeclaration obj &&
                   Equals(obj.Value, Value);
        }
    }
}

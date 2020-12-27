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
    }
}

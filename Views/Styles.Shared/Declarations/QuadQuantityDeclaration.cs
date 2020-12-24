using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Das.Views.Styles.Declarations
{
    public abstract class QuadQuantityDeclaration : DeclarationBase
    {
        public QuadQuantityDeclaration(String value,
                                       IStyleVariableAccessor variableAccessor, 
                                       DeclarationProperty property,
                                       DeclarationProperty topProperty,
                                       DeclarationProperty rightProperty,
                                       DeclarationProperty bottomProperty,
                                       DeclarationProperty leftProperty) 
            : base(variableAccessor, property)
        {
            var tokens = value.Split();

            switch (tokens.Length)
            {
                case 4:
                    Top = new QuantityDeclaration(tokens[0], variableAccessor, topProperty);
                    Right = new QuantityDeclaration(tokens[1], variableAccessor, rightProperty);
                    Bottom = new QuantityDeclaration(tokens[2], variableAccessor, bottomProperty);
                    Left = new QuantityDeclaration(tokens[3], variableAccessor, leftProperty);
                    break;
                
                case 3:
                    Top = new QuantityDeclaration(tokens[0], variableAccessor, topProperty);
                    
                    Right = new QuantityDeclaration(tokens[1], variableAccessor, rightProperty);
                    Left = new QuantityDeclaration(tokens[1], variableAccessor, leftProperty);
                    
                    Bottom = new QuantityDeclaration(tokens[2], variableAccessor, bottomProperty);
                    break;
                
                case 2:
                    Top = new QuantityDeclaration(tokens[0], variableAccessor, topProperty);
                    Bottom = new QuantityDeclaration(tokens[0], variableAccessor, bottomProperty);
                    
                    Right = new QuantityDeclaration(tokens[1], variableAccessor, rightProperty);
                    Left = new QuantityDeclaration(tokens[1], variableAccessor, leftProperty);
                    break;
                
                case 1:
                    Top = new QuantityDeclaration(tokens[0], variableAccessor, topProperty);
                    Bottom = new QuantityDeclaration(tokens[0], variableAccessor, bottomProperty);
                    Right = new QuantityDeclaration(tokens[0], variableAccessor, rightProperty);
                    Left = new QuantityDeclaration(tokens[0], variableAccessor, leftProperty);
                    break;
                
                default:
                    throw new NotImplementedException();
            }
        }

        public QuantityDeclaration Left { get; }
        
        public QuantityDeclaration Top { get; }
        
        public QuantityDeclaration Right { get; }
        
        public QuantityDeclaration Bottom { get; }
    }
}

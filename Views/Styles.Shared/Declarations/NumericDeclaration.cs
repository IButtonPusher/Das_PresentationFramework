using System;

namespace Das.Views.Styles.Declarations
{
    /// <summary>
    /// A single number without quantity
    /// </summary>
    public class NumericDeclaration : DeclarationBase
    {
        public NumericDeclaration(String value,
            IStyleVariableAccessor variableAccessor, 
                                  DeclarationProperty property) 
            : base(variableAccessor, property)
        {
            Value = Double.TryParse(value, out var val) ? val : Double.NaN;
        }
        
        public NumericDeclaration(Double value,
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

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Double Value { get; }
    }
}

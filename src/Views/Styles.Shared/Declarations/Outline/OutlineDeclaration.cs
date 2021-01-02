using System;
using Das.Views.BoxModel;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Das.Views.Styles.Declarations
{
    public class OutlineDeclaration : DeclarationBase
    {
        public OutlineDeclaration(String value,
            IStyleVariableAccessor variableAccessor) 
            : base(variableAccessor, DeclarationProperty.Outline)
        {

            var tokens = value.Split();

            for (var c = 0; c < tokens.Length; c++)
            {
                var token = tokens[c];

                if (Style == null)
                {
                    var styleIs = GetEnumValue(token, OutlineStyle.Invalid);
                    if (styleIs != OutlineStyle.Invalid)
                    {
                        Style = new OutlineStyleDeclaration(token, variableAccessor);
                        continue;
                    }
                }

                if (Width == null)
                {
                    if (QuantityDeclaration.IsValidQuantity(token))
                    {
                        Width = new OutlineWidthDeclaration(token, variableAccessor);
                        continue;
                    }
                }

                Color = new OutlineColorDeclaration(value, variableAccessor);

            }

            if (Style == null)
                throw new InvalidOperationException();

        }
        
        public OutlineStyleDeclaration Style { get; }
        
        public OutlineColorDeclaration? Color { get; }
        
        public OutlineWidthDeclaration? Width { get; }
    }
}

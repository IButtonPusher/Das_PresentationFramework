using System;
using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Das.Views.Styles.Declarations
{
    public class BoxShadow
    {
        public BoxShadow(String value,
                         IStyleVariableAccessor variableAccessor)
        {
            List<String> tokens;
            
            var rgbIndex = value.IndexOf("rgb", StringComparison.OrdinalIgnoreCase);
            if (rgbIndex < 0)
            {
                tokens = new List<String>(value.Split());
            }
            else
            {
                var preRgb = value.Substring(0, rgbIndex - 1);
                tokens = new List<String>(preRgb.Split());

                var withRgb = value.Substring(rgbIndex);
                tokens.Add(withRgb);
            }

            switch (tokens.Count)
            {
                case 5:
                    SpreadRadius = new QuantityDeclaration(tokens[3],
                        variableAccessor, DeclarationProperty.BoxShadow);
                    goto case 4;

                case 4:
                    BlurRadius = new QuantityDeclaration(tokens[2],
                        variableAccessor, DeclarationProperty.BoxShadow);
                    break;

                //case 3:
                //    OffsetY = new QuantityDeclaration(tokens[1],
                //        variableAccessor, DeclarationProperty.BoxShadow);

                //    goto case 2;

                //case 2:
                //    OffsetX = new QuantityDeclaration(tokens[0],
                //        variableAccessor, DeclarationProperty.BoxShadow);
                //    goto case 1;

                //case 1:
                //    Color = new ColorDeclaration(tokens[tokens.Count - 1], DeclarationProperty.BoxShadow,
                //        variableAccessor);
                //    break;
            }

            OffsetY = new QuantityDeclaration(tokens[1],
                variableAccessor, DeclarationProperty.BoxShadow);
            
            OffsetX = new QuantityDeclaration(tokens[0],
                variableAccessor, DeclarationProperty.BoxShadow);
            
            Color = new ColorDeclaration(tokens[tokens.Count - 1], DeclarationProperty.BoxShadow,
                variableAccessor);
            
        }
        
        public QuantityDeclaration OffsetX { get; }
        
        public QuantityDeclaration OffsetY { get; }
        
        public QuantityDeclaration? BlurRadius {get;}

        public QuantityDeclaration? SpreadRadius { get; }
        
        public ColorDeclaration Color {get;}
    }
}

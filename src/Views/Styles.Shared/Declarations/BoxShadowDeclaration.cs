using System;
using System.Collections.Generic;
using System.Linq;
using Das.Views.BoxModel;
using Das.Views.Core.Drawing;

namespace Das.Views.Styles.Declarations
{
    public class BoxShadowDeclaration : ValueDeclaration<BoxShadow>
    {
        public BoxShadowDeclaration(String value,
            IStyleVariableAccessor variableAccessor) 
            : base(GetBoxShadow(value, variableAccessor),
                variableAccessor, DeclarationProperty.BoxShadow)
        {
           
        }

        private static BoxShadow GetBoxShadow(String value,
                                              IStyleVariableAccessor variableAccessor)
        {
            var layers = GetShadowLayers(value, variableAccessor);
            return new BoxShadow(layers);
        }

        private static IEnumerable<BoxShadowLayer> GetShadowLayers(String value,
                                                                   IStyleVariableAccessor variableAccessor)
        {
            var multiTokens = GetMultiSplit(value, ',');

            foreach (var multiToken in multiTokens)
            {
                var tokens = GetMultiSplit(multiToken, ' ').ToArray();

                if (tokens.Length < 2)
                    throw new InvalidOperationException();

                var offsetX = GetQuantity(tokens[0], variableAccessor);
                var offsetY = GetQuantity(tokens[1], variableAccessor);
                var blur = QuantifiedDouble.Zero;
                var spread = QuantifiedDouble.Zero;
                IBrush color = SolidColorBrush.Black;
                var isInset = false;

                for (var c = 2; c < tokens.Length; c++)
                {
                    //if (tokens.Length > 2)
                    {
                        if (TryGetQuantity(tokens[c], variableAccessor, out var q))
                        {
                            if (blur.IsZero())
                                blur = q;
                            else
                                spread = q;
                        }

                        else if (TryGetColor(tokens[c], variableAccessor, out var clr))
                            color = clr;

                        else if (String.Equals(tokens[c], nameof(BoxShadowLayer.IsInset),
                            StringComparison.OrdinalIgnoreCase))
                            isInset = true;

                        else
                            throw new NotImplementedException();
                    }
                }

                yield return new BoxShadowLayer(offsetX, offsetY, blur, spread, color, isInset);
            }


            //foreach (var token in tokens)
            //    yield return new BoxShadow(token, variableAccessor);
        }

        //private readonly List<BoxShadow> _shadows;

        //public IEnumerable<BoxShadow> Shadows => _shadows;
    }
}

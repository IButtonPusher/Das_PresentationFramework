using System;
using System.Threading.Tasks;
using Das.Views.Styles.Functions;

namespace Das.Views.Styles.Declarations
{
    public class FontFamilyDeclaration : DeclarationBase
    {
        public FontFamilyDeclaration(String fontFamily,
                                     IStyleVariableAccessor variableAccessor)
            : base(variableAccessor, DeclarationProperty.FontFamily)
        {
            var fn = FunctionBuilder.GetFunction(fontFamily, variableAccessor);

            var val = fn.GetValue();

            switch (val)
            {
                case String str:
                    _fontFamily = str;
                    break;

                case Object?[] fallbacks:

                    foreach (var fallback in fallbacks)
                        if (fallback is String strValid)
                        {
                            //todo: ensure the font face exists + is valid etc
                            _fontFamily = strValid;
                            break;
                        }

                    break;
            }


            _fontFamily = fontFamily;
        }

        private readonly String _fontFamily;
    }
}
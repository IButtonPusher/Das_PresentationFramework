using System;
using System.Threading.Tasks;
using Das.Views.Styles.Functions;

namespace Das.Views.Styles.Declarations
{
    public class FontFamilyDeclaration : ValueDeclaration<String>
    {
        public FontFamilyDeclaration(String fontFamily,
                                     IStyleVariableAccessor variableAccessor)
            : base(GetFontFamily(fontFamily, variableAccessor),
                variableAccessor, DeclarationProperty.FontFamily)
        {
           
        }
        
        private static String GetFontFamily(String fontFamily,
                                            IStyleVariableAccessor variableAccessor)
        {
            var fn = FunctionBuilder.GetFunction(fontFamily, variableAccessor);

            var val = fn.GetValue();

            switch (val)
            {
                case String str:
                    return str;

                case Object?[] fallbacks:

                    foreach (var fallback in fallbacks)
                        if (fallback is String strValid)
                        {
                            //todo: ensure the font face exists + is valid etc
                            return strValid;
                        }

                    break;
            }

            return fontFamily;
        }

        //private readonly String _fontFamily;
    }
}
using System;
using Das.Views.Core.Drawing;
using Das.Views.Styles.Functions;

namespace Das.Views.Styles.Declarations
{
    public class ColorDeclaration : ValueDeclaration<IBrush?>
    {
        public ColorDeclaration(String value,
                                DeclarationProperty property,
                                IStyleVariableAccessor variableAccessor)
            : base(GetBrush(value, variableAccessor),
                variableAccessor, property)
        {
            //var fnCall2 = FunctionBuilder.GetFunction(value, variableAccessor);
            //var fnVal2 = fnCall2.GetValue();
            //_brush = fnVal2 as IBrush;
        }

        private static IBrush? GetBrush(String value,
                                        IStyleVariableAccessor variableAccessor)
        {
            var fnCall = FunctionBuilder.GetFunction(value, variableAccessor);
            var fnVal = fnCall.GetValue();
            return fnVal as IBrush;
        }
        
        //public override String ToString()
        //{
        //    return "Color: " + Property + " - " + _brush;
        //}

        //private readonly IBrush? _brush;


    }
}

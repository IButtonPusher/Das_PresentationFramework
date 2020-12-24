using System;
using Das.Views.Core.Drawing;
using Das.Views.Styles.Functions;

namespace Das.Views.Styles.Declarations
{
    public class ColorDeclaration : DeclarationBase
    {
        public ColorDeclaration(String value,
                                DeclarationProperty property,
                                IStyleVariableAccessor variableAccessor)
            : base(variableAccessor, property)
        {
            var fnCall2 = FunctionBuilder.GetFunction(value, variableAccessor);
            var fnVal2 = fnCall2.GetValue();
            _brush = fnVal2 as IBrush;
            
            if (_brush == null)
            {}

        }

        public override String ToString()
        {
            return "Color: " + Property + " - " + _brush;
        }

        private readonly IBrush? _brush;


    }
}

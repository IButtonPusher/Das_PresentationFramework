using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Styles.Declarations
{
    public class VerticalAlignDeclaration : EnumDeclaration<VerticalAlignType>
    {
        public VerticalAlignDeclaration(String value,
                                        IStyleVariableAccessor variableAccessor) 
            : base(value, VerticalAlignType.Baseline, variableAccessor,  
                DeclarationProperty.VerticalAlign)
        {
        }
    }
}

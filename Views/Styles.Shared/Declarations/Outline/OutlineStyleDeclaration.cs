﻿using System;

namespace Das.Views.Styles.Declarations.Outline
{
    public class OutlineStyleDeclaration : EnumDeclaration<OutlineStyle>
    {
        public OutlineStyleDeclaration(String value, 
                                       IStyleVariableAccessor variableAccessor) 
            : base(value, OutlineStyle.None, variableAccessor, DeclarationProperty.OutlineStyle)
        {
        }
    }
}

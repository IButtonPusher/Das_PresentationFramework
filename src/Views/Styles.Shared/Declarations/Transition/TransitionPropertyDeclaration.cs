﻿using System;

namespace Das.Views.Styles.Declarations.Transition;

public class TransitionPropertyDeclaration : EnumDeclaration<DeclarationProperty>
{
   public TransitionPropertyDeclaration(String value, 
                                        DeclarationProperty defaultValue/*, 
                                        IStyleVariableAccessor variableAccessor*/) 
      : base(value, defaultValue, /*variableAccessor, */DeclarationProperty.TransitionProperty)
   {
   }
}
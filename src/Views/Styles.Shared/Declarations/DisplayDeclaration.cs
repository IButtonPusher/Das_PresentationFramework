using System;

namespace Das.Views.Styles.Declarations
{
    public class DisplayDeclaration : EnumDeclaration<DisplayType>
    {
        public DisplayDeclaration(DisplayType displayType,
                                  IStyleVariableAccessor variableAccessor)
        : base(displayType, variableAccessor, DeclarationProperty.Display)
        {
            
        }

        public DisplayDeclaration(String displayName,
                                  IStyleVariableAccessor variableAccessor)
            : this(GetEnumValue(displayName, DisplayType.Initial),
                variableAccessor)
        {

        }
    }
}

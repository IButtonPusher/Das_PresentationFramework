using System;

namespace Das.Views.Styles.Declarations
{
    public class PositionDeclaration : EnumDeclaration<PositionType>
    {
        //private readonly PositionType _positionType;

        public PositionDeclaration(String positionName,
                                   IStyleVariableAccessor variableAccessor) 
        : this(GetEnumValue(positionName, PositionType.Initial), 
            variableAccessor)
        {
            
        }

        public PositionDeclaration(PositionType positionType,
                                   IStyleVariableAccessor variableAccessor)
        : base(positionType, variableAccessor, DeclarationProperty.Position)
        {
          //  _positionType = positionType;
        }
    }
}

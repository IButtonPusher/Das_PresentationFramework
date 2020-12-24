using System;

namespace Das.Views.Styles.Declarations
{
    public class DisplayDeclaration : DeclarationBase
    {
        private readonly DisplayType _displayType;

        public DisplayDeclaration(DisplayType displayType,
                                  IStyleVariableAccessor variableAccessor)
        : base(variableAccessor, DeclarationProperty.Display)
        {
            _displayType = displayType;
        }

        public DisplayDeclaration(String displayName,
                                  IStyleVariableAccessor variableAccessor)
            : this(GetEnumValue(displayName, DisplayType.Initial),
                variableAccessor)
        {

        }


        //public override void AssignValueToVisual(IVisualElement visual)
        //{
        //    throw new NotImplementedException();
        //}
        
    }
}

using System;
using Das.Views.Declarations;

namespace Das.Views.Styles.Declarations
{
    public class AppearanceDeclaration : EnumDeclaration<AppearanceType>
    {
        public AppearanceDeclaration(String value,
                                     IStyleVariableAccessor variableAccessor)
            : base(value, AppearanceType.Auto, variableAccessor, DeclarationProperty.Appearance)
        {
        }
    }
}

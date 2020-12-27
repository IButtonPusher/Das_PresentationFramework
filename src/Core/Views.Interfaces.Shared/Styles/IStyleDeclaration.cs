using System;
using Das.Views.Styles.Declarations;

namespace Das.Views.Styles
{
    public interface IStyleDeclaration
    {
        DeclarationProperty Property { get; }
    }
}

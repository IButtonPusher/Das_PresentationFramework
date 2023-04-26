using System;
using Das.Views.Styles.Declarations;

namespace Das.Views.Styles;

public interface IStyleDeclaration : IEquatable<IStyleDeclaration>
{
   DeclarationProperty Property { get; }
}
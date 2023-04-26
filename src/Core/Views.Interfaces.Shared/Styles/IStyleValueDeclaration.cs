using System;

namespace Das.Views.Styles;

public interface IStyleValueDeclaration : IStyleDeclaration
{
   Object? Value {get;}
}
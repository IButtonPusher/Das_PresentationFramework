using System;

namespace Das.Views.Styles;

public interface IStyleValueDeclaration<out T> : IStyleValueDeclaration
{
   new T Value { get; }
}
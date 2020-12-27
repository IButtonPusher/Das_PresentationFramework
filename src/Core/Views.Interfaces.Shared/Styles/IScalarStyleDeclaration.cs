using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Styles
{
    public interface IScalarStyleDeclaration : IStyleDeclaration
    {
        Object? Value { get; }
    }
}

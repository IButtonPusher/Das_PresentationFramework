using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Styles
{
    public interface IStyleValueDeclaration : IStyleDeclaration
    {
        Object? Value {get;}
    }
}

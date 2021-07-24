using System;
using System.Collections.Generic;

namespace Das.Views.Styles
{
    public interface IStyleMultiValueDeclaration<out T> : IStyleDeclaration
    {
        IEnumerable<T> Values { get; }
    }
}

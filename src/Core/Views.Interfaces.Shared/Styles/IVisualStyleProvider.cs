using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Styles
{
    public interface IVisualStyleProvider
    {
        IAsyncEnumerable<IStyleRule> GetStylesByClassNameAsync(String className);
    }
}

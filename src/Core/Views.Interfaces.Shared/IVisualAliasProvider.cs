using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views
{
    public interface IVisualAliasProvider
    {
        Type? GetVisualTypeFromAlias(String alias);
    }
}

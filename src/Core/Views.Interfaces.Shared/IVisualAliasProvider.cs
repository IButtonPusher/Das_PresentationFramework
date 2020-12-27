using System;

namespace Das.Views
{
    public interface IVisualAliasProvider
    {
        Type? GetVisualTypeFromAlias(String alias);
    }
}

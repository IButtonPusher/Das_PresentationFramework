using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Das.Views.Construction
{
    public interface IResourceBuilder
    {
        Task<Object?> GetEmbeddedResourceAsync(String path,
                                               String[] pathTokens,
                                               Assembly assembly);

        Object? GetEmbeddedResource(String path,
                                    String[] pathTokens,
                                    Assembly assembly);
    }
}

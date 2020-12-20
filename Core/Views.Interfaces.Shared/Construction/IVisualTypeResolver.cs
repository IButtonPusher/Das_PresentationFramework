using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Construction
{
    public interface IVisualTypeResolver
    {
        Dictionary<String, String> GetNamespaceAssemblySearch(
            IMarkupNode node,
            IDictionary<String, String> search);

        Type GetType(IMarkupNode node,
                     String? genericArgName,
                     Dictionary<String, String> nameSpaceAssemblySearch);
    }
}

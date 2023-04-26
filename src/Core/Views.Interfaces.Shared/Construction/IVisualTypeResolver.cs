using System;
using System.Collections.Generic;

namespace Das.Views.Construction;

public interface IVisualTypeResolver
{
   Dictionary<String, String> GetNamespaceAssemblySearch(
      IMarkupNode node,
      IDictionary<String, String> search);

   Type GetType(IMarkupNode node,
                String? genericArgName);

   Type GetType(String name);
        
   Type GetType(String name,
                Dictionary<String, String> nameSpaceAssemblySearch);
        
   Type GetType(IMarkupNode node,
                String? genericArgName,
                Dictionary<String, String> nameSpaceAssemblySearch);
}
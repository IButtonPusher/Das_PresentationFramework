using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Serializer;

namespace Das.Views.Construction
{
    public class VisualTypeResolver : IVisualTypeResolver
    {
        public VisualTypeResolver(ITypeInferrer typeInferrer)
        {
            _typeInferrer = typeInferrer;
        }

        public Dictionary<String, String> GetNamespaceAssemblySearch(IMarkupNode node,
                                                                     IDictionary<String, String> search)
        {
            var nsAsmSearch = new Dictionary<String, String>(search);
            return GetNamespaceAssemblySearchImpl(node, nsAsmSearch);
        }

        public Type GetType(IMarkupNode node, String? genericArgName,
                            Dictionary<String, String> nameSpaceAssemblySearch)
        {
            var name = node.Name;

            var notGeneric = _typeInferrer.GetTypeFromClearName(name, nameSpaceAssemblySearch);
            if (notGeneric != null)
                return notGeneric;

            if (!String.IsNullOrEmpty(genericArgName) && node.ChildrenCount > 0)
            {
                var letsTry = name + "[" + genericArgName + "]";
                var found = _typeInferrer.GetTypeFromClearName(letsTry, nameSpaceAssemblySearch, true);
                if (found != null)
                    return found;
            }

            throw new TypeLoadException(name);
        }

        private static Dictionary<String, String> GetNamespaceAssemblySearchImpl(IMarkupNode node,
            Dictionary<String, String> nsAsmSearch)
        {
            foreach (var attribute in node.GetAllAttributes())
            {
                if (!attribute.Key.StartsWith("xmlns:"))
                    continue;
                var tokens = attribute.Value.Split(';');


                String? asmName = null;
                String? ns = null;

                for (var t = 0; t < tokens.Length; t++)
                {
                    var subTokens = tokens[t].Split(_namespaceSplitters);
                    if (subTokens.Length != 2)
                        continue;

                    switch (subTokens[0])
                    {
                        case "clr-namespace":
                            ns = subTokens[1];
                            break;

                        case "assembly":
                            asmName = subTokens[1];
                            break;
                    }
                }

                if (ns == null)
                    continue;

                if (asmName == null)
                    continue;

                nsAsmSearch[ns] = asmName;
            }

            return nsAsmSearch;
        }

        private static readonly Char[] _namespaceSplitters = {':', '='};
        private readonly ITypeInferrer _typeInferrer;
    }
}
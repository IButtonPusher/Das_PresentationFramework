using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Controls;
using Das.Views.Primitives;

namespace Das.Views.Construction
{
    public class VisualTypeResolver : IVisualTypeResolver
    {
        public VisualTypeResolver(ITypeInferrer typeInferrer,
                                  IDictionary<String, VisualTypeTuple> typeAliases)

        {
            _typeInferrer = typeInferrer;
            _typeAliases = typeAliases;
        }
        
        public VisualTypeResolver(ITypeInferrer typeInferrer) 
            :  this(typeInferrer, _defaultAliases)
        {
            
        }

        static VisualTypeResolver()
        {
            _defaultNamespaceSeed = new Dictionary<String, String>();
            _defaultNamespaceSeed["Das.Views.Panels"] = "Das.Views";
            _defaultNamespaceSeed["Das.Views.Controls"] = "Das.Views";
            _defaultNamespaceSeed["Das.Views.DataBinding"] = "Das.Views";
            _defaultNamespaceSeed["Das.Views.Templates"] = "Das.Views";
            _defaultNamespaceSeed["Das.Views.Primitives"] = "Das.Views";

            _defaultNamespaceSeed["Das.Views"] = "Das.Views";

            _defaultAliases = new Dictionary<String, VisualTypeTuple>();
            _defaultAliases["label"] = new VisualTypeTuple(typeof(Label));
            _defaultAliases["span"] = new VisualTypeTuple(typeof(SpanSlim), typeof(Span));
        }
        
        public Dictionary<String, String> GetNamespaceAssemblySearch(IMarkupNode node,
                                                                     IDictionary<String, String> search)
        {
            var nsAsmSearch = new Dictionary<String, String>(search);
            return GetNamespaceAssemblySearchImpl(node, nsAsmSearch);
        }

        public Type GetType(IMarkupNode node, 
                            String? genericArgName)
        {
            return GetType(node, genericArgName, _defaultNamespaceSeed);
        }

        public Type GetType(String name)
        {
            return GetType(name, _defaultNamespaceSeed);
        }

        public Type GetType(String name, 
                            Dictionary<String, String> nameSpaceAssemblySearch)
        {
            var notGeneric = _typeInferrer.GetTypeFromClearName(name, nameSpaceAssemblySearch);

            return notGeneric ?? throw new TypeLoadException(name);
        }

        public Type GetType(IMarkupNode node, 
                            String? genericArgName,
                            Dictionary<String, String> nameSpaceAssemblySearch)
        {
            var name = node.Name;

            if (_typeAliases.TryGetValue(name, out var aliasedType))
            {
                return node.ChildrenCount == 0 
                    ? aliasedType.ChildlessType
                    : aliasedType.PanelType;
            }

            if (name == "input" && node.TryGetAttributeValue("type", out var inputType))
            {
                switch (inputType)
                {
                    case "checkbox":
                        return typeof(CheckBox);
                }
            }

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
        private readonly IDictionary<String, VisualTypeTuple> _typeAliases;
        private static readonly Dictionary<String, String> _defaultNamespaceSeed;
        private static readonly Dictionary<String, VisualTypeTuple> _defaultAliases;
    }
}
using System;
using System.Collections.Generic;
using Das.Views.Input;
using Das.Views.Primitives;

namespace Das.Views.Construction
{
    public class VisualAliasProvider : IVisualAliasProvider
    {

        private static readonly Dictionary<String, Type> _defaultMappings;
        private readonly Dictionary<String, Type> _typeMappings;

        static VisualAliasProvider()
        {
            _defaultMappings = new Dictionary<String, Type>
            {
                {"input", typeof(IInputVisual)},
                {"span", typeof(Span)}
            };
        }

        public VisualAliasProvider()
        {
            _typeMappings = new Dictionary<String, Type>(_defaultMappings);
        }
        
        public VisualAliasProvider(IDictionary<String, Type> nonDefaultMappings) : this()
        {
            foreach (var kvp in nonDefaultMappings)
                _typeMappings[kvp.Key] = kvp.Value;
        }

        public Type? GetVisualTypeFromAlias(String alias)
        {
            if (_typeMappings.TryGetValue(alias, out var found))
                return found;

            return default;
        }
    }
}

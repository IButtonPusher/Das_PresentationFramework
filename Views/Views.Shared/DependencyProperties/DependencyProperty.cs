using System;
using System.Collections.Generic;

namespace Das.Views
{
    public static class DependencyProperty
    {
        public static void NotifyTypeRegistration<TVisual, TValue>(
            DependencyProperty<TVisual, TValue> dependencyProperty)
            where TVisual : IVisualElement
        {
            lock (_lock)
            {
                if (!_typeProps.TryGetValue(typeof(TVisual), out var items))
                {
                    items = new List<IDependencyProperty>();
                    _typeProps.Add(typeof(TVisual), items);
                }

                items.Add(dependencyProperty);
            }
        }

        public static IEnumerable<IDependencyProperty> GetDependencyPropertiesForType(Type type)
        {
            return GetDependencyPropertiesForTypeImpl(type, true);
        }
        
        
        private static IEnumerable<IDependencyProperty> GetDependencyPropertiesForTypeImpl(Type type,
                Boolean isTryInterfaces)
        {
            lock (_lock)
            {
                if (_typeProps.TryGetValue(type, out var items))
                {
                    foreach (var prop in items)
                        yield return prop;
                }

                if (type.BaseType != null && typeof(IVisualElement).IsAssignableFrom(type.BaseType))
                {
                    foreach (var bdp in GetDependencyPropertiesForTypeImpl(type.BaseType, false))
                        yield return bdp;
                }

                if (!isTryInterfaces)
                    yield break;

                foreach (var ivisual in type.GetInterfaces())
                {
                    if (!typeof(IVisualElement).IsAssignableFrom(ivisual))
                        continue;
                    
                    foreach (var bdp in GetDependencyPropertiesForTypeImpl(ivisual, false))
                        yield return bdp;
                }
                
            }
        }
        
        
        private static readonly Object _lock = new Object();

        private static readonly Dictionary<Type, List<IDependencyProperty>> _typeProps =
            new Dictionary<Type, List<IDependencyProperty>>();
    }
}

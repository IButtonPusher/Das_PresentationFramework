using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Das.Views.Data;
using Das.Views.Transitions;

namespace Das.Views
{
    public abstract class DependencyProperty : IDependencyProperty
    {
        public static void NotifyTypeRegistration<TVisual, TValue>(
            DependencyProperty<TVisual, TValue> dependencyProperty)
            where TVisual : IVisualElement
        {
            lock (_lock)
            {
                if (!_typeProps.TryGetValue(typeof(TVisual), out var items))
                {
                    items = new Dictionary<String, IDependencyProperty>();
                    _typeProps.Add(typeof(TVisual), items);
                }

                items.Add(dependencyProperty.Name, dependencyProperty);
            }
        }

        public static void NotifyTypeRegistration<TValue>(DependencyProperty<TValue> dependencyProperty,
                                                          Type ownerType)

        {
            lock (_lock)
            {
                if (!_typeProps.TryGetValue(ownerType, out var items))
                {
                    items = new Dictionary<String, IDependencyProperty>();
                    _typeProps.Add(ownerType, items);
                }

                items.Add(dependencyProperty.Name, dependencyProperty);
            }
        }

        public static Boolean TryGetDependecyProperty(Type type,
                                                      String name,
                                                      out IDependencyProperty dependencyProperty)
        {
            return TryGetDependecyPropertyImpl(type, name, true, out dependencyProperty);
        }

        public static Boolean TryGetDependecyPropertyImpl(Type type,
                                                          String name,
                                                          Boolean isTryInterfaces,
                                                          out IDependencyProperty dependencyProperty)
        {
            EnsureStaticConstructor(type);

            lock (_lock)
            {
                if (_typeProps.TryGetValue(type, out var items) &&
                    items.TryGetValue(name, out dependencyProperty))
                    return true;
            }

            if (type.BaseType != null && typeof(IVisualElement).IsAssignableFrom(type.BaseType))
            {
                if (TryGetDependecyPropertyImpl(type.BaseType, name, false, out dependencyProperty))
                    return true;
            }

            if (!isTryInterfaces)
                goto fail;

            foreach (var ivisual in type.GetInterfaces())
            {
                if (!typeof(IVisualElement).IsAssignableFrom(ivisual))
                    continue;

                if (TryGetDependecyPropertyImpl(ivisual, name, false, out dependencyProperty))
                    return true;
            }

            fail:
            dependencyProperty = default!;
            return false;
        }

        public static IEnumerable<IDependencyProperty> GetDependencyPropertiesForType(Type type)
        {
            return GetDependencyPropertiesForTypeImpl(type, true);
        }

        private static void EnsureStaticConstructor(Type type)
        {
            if (!type.IsInterface)
            {
                // ensure cctor has run
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }
        }

        private static IEnumerable<IDependencyProperty> GetDependencyPropertiesForTypeImpl(Type type,
            Boolean isTryInterfaces)
        {
            lock (_lock)
            {
                EnsureStaticConstructor(type);

                if (_typeProps.TryGetValue(type, out var items))
                {
                    foreach (var prop in items)
                        yield return prop.Value;
                }

                if (type.BaseType != null && typeof(IVisualElement).IsAssignableFrom(type.BaseType))
                {
                    foreach (var bdp in GetDependencyPropertiesForTypeImpl(type.BaseType, false))
                        yield return bdp;
                }

                if (!isTryInterfaces)
                    yield break;

                // INTERFACES

                foreach (var ivisual in type.GetInterfaces())
                {
                    if (!typeof(IVisualElement).IsAssignableFrom(ivisual))
                        continue;

                    foreach (var bdp in GetDependencyPropertiesForTypeImpl(ivisual, false))
                        yield return bdp;
                }
            }
        }

        public static readonly Object UnsetValue = new NamedObject("DependencyProperty.UnsetValue");


        private static readonly Object _lock = new Object();

        private static readonly Dictionary<Type, Dictionary<String, IDependencyProperty>> _typeProps =
            new Dictionary<Type, Dictionary<String, IDependencyProperty>>();

        public abstract Boolean Equals(IDependencyProperty other);

        public abstract String Name { get; }

        public abstract Object? GetValue(IVisualElement visual);

        public abstract void SetValue(IVisualElement visual,
                                      Object? value);

        public abstract void SetValueNoTransitions(IVisualElement visual,
                                                   Object? value);

        public abstract void SetValueFromStyle(IVisualElement visual,
                                               Object? value);

        public abstract void ClearValue(IVisualElement visual);

        public abstract void SetComputedValueFromStyle(IVisualElement visual,
                                                       Func<IVisualElement, Object?> value);

        public abstract void AddOnChangedHandler(IVisualElement visual,
                                                 Action<IDependencyProperty> onChange);

        public abstract void AddTransition(IVisualElement visual,
                                           ITransition transition);

        public abstract Object? DefaultValue { get; }

        public abstract Type PropertyType { get; }

        public abstract Type VisualType { get; }

        public virtual Boolean IsReadOnly => false;
    }
}

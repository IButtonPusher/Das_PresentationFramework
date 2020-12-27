using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Styles;

namespace Das.Views
{
    public class DependencyProperty<TVisual, TValue> : IDependencyProperty<TVisual, TValue>,
                                                       IDependencyProperty
        where TVisual : IVisualElement
    {
        public DependencyProperty(String propertyName,
                                  TValue defaultValue)
        {
            _delegateLock = new Object();
            _propertyName = propertyName;
            _defaultValue = defaultValue;
            
            _values = new ConcurrentDictionary<TVisual, TValue>();
            _computedValues = new ConcurrentDictionary<TVisual, Func<IVisualElement, Object?>>();
            
            _changings = new ConcurrentDictionary<TVisual, List<Func<TVisual, TValue, TValue, Boolean>>>();
            _changeds = new ConcurrentDictionary<TVisual, List<Action<TVisual, TValue, TValue>>>();
            _knownVisuals = new ConcurrentDictionary<TVisual, Byte>();
            _staticChangeds = new List<Action<TVisual, TValue, TValue>>();
        }

        Object? IDependencyProperty.GetValue(IVisualElement visual)
        {
            if (visual is TVisual tv)
                return GetValue(tv);

            throw new InvalidCastException(visual + " cannot be cast to type " + typeof(TVisual));
        }

        void IDependencyProperty.SetValue(IVisualElement visual,
                                          Object? value)
        {
            SetRuntimeValueImpl(visual, value, false);
        }

        private static TOut GetValue<TIn, TOut>(TIn value)
        {
            if (Equals(value, default))
                return default!;
            
            if (!(value is TOut tv))
                throw new InvalidCastException(value + " cannot be cast to type " + typeof(TOut));

            return tv;
        }

        private void SetRuntimeValueImpl(IVisualElement visual,
                                         Object? value,
                                         Boolean isFromStyle)
        {
            var tv = GetValue<IVisualElement, TVisual>(visual);

            switch (value)
            {
                case null:
                    SetValue(tv, default!);
                    break;

                case TValue valid:
                    SetValue(tv, valid);
                    break;

                default:
                    throw new InvalidCastException(value + " cannot be cast to type " + typeof(TValue));
            }
        }

        void IDependencyProperty.SetValueFromStyle(IVisualElement visual, Object? value)
        {
            SetRuntimeValueImpl(visual, value, true);
        }

        void IDependencyProperty.SetComputedValueFromStyle(IVisualElement visual, 
                                                           Func<IVisualElement, Object?> value)
        {
            var tv = GetValue<IVisualElement, TVisual>(visual);
            _values.TryRemove(tv, out _);
            
            _computedValues[tv] = value;
            EnsureKnown(tv);
        }

        public String Name => _propertyName;

        public Type PropertyType => typeof(TValue);

        public Type VisualType => typeof(TVisual);

        public void AddOnChangedHandler(Action<TVisual, TValue, TValue> handler)
        {
            lock (_delegateLock)
            {
                _staticChangeds.Add(handler);
            }
        }

        public void AddOnChangedHandler(TVisual forVisual,
                                        Action<TVisual, TValue, TValue> handler)
        {
            var items = _changeds.GetOrAdd(forVisual, GetChangedDelegate);
            lock (_delegateLock)
            {
                items.Add(handler);
            }
        }

        public void AddOnChangingHandler(TVisual forVisual,
                                         Func<TVisual, TValue, TValue, Boolean> handler)
        {
            var items = _changings.GetOrAdd(forVisual, GetChangingDelegate);
            lock (_delegateLock)
            {
                items.Add(handler);
            }
        }

        private Boolean TryGetKnownOrComputedValue(TVisual forVisual,
                                                   out TValue value)
        {
            if (_values.TryGetValue(forVisual, out value))
                return true;
            
            if (_computedValues.TryGetValue(forVisual, out var computer))
            {
                var computed = computer(forVisual);
                value = GetValue<Object, TValue>(computed!);
                return true;
            }
            
            value = default!;
            return false;
        }

        public TValue GetValue(TVisual forVisual)
        {
            if (!TryGetKnownOrComputedValue(forVisual, out var good))
            {
                good = _defaultValue;
                EnsureKnown(forVisual);
            }
            
            return good;
        }

        public TValue GetValue(TVisual forVisual,
                               IStyleProvider contextStyle,
                               Func<TVisual, IStyleProvider, TValue> getDefault)
        {
            //if (!_values.TryGetValue(forVisual, out var good))
            if (!TryGetKnownOrComputedValue(forVisual, out var good))
            {
                good = getDefault(forVisual, contextStyle);
                EnsureKnown(forVisual);
            }

            return good;
        }

        public void SetValue(TVisual forVisual,
                             TValue value,
                             Func<TValue, TValue, Boolean> onChanging,
                             Action<TValue, TValue> onChanged)
        {
            SetValueImpl(forVisual, value,
                GetOnChanging(forVisual, WrapOnChanging(forVisual, onChanging)),
                GetOnChanged(forVisual, WrapOnChanged(forVisual, onChanged)));
        }
        
        public void SetValue(TVisual forVisual,
                             TValue value,
                             Action<TValue, TValue> onChanged)
        {
            SetValueImpl(forVisual, value,
                GetOnChanging(forVisual, null),
                GetOnChanged(forVisual, WrapOnChanged(forVisual, onChanged)));
        }
        

        public void SetValue(TVisual forVisual,
                             TValue value,
                             Func<TVisual, TValue, TValue, Boolean> onChanging,
                             Action<TVisual, TValue, TValue> onChanged)
        {
            SetValueImpl(forVisual, value,
                GetOnChanging(forVisual, onChanging),
                GetOnChanged(forVisual, onChanged));
        }


        public void SetValue(TVisual forVisual,
                             TValue value)
        {
            SetValueImpl(forVisual, value,
                GetOnChanging(forVisual, null),
                GetOnChanged(forVisual, null));
        }

        public override String ToString()
        {
            return _propertyName;
        }

        public static DependencyProperty<TVisual, TValue> Register(String propertyName,
                                                                   TValue defaultValue,
                                                                   Action<TVisual, TValue, TValue> onChanged)
        {
            var res = Register(propertyName, defaultValue);
            res.AddOnChangedHandler(onChanged);
            return res;
        }

        public static DependencyProperty<TVisual, TValue> Register(String propertyName,
                                                                   TValue defaultValue)
        {
            var dep = new DependencyProperty<TVisual, TValue>(propertyName, defaultValue);

            DependencyProperty.NotifyTypeRegistration(dep);

            return dep;
        }

        private void EnsureKnown(TVisual forVisual)
        {
            if (!_knownVisuals.TryGetValue(forVisual, out _))
            {
                forVisual.Disposed += OnVisualDisposed;
                _knownVisuals[forVisual] = 0;
            }
        }

        private List<Action<TVisual, TValue, TValue>> GetChangedDelegate(TVisual forVisual)
        {
            EnsureKnown(forVisual);
            return new List<Action<TVisual, TValue, TValue>>();
        }

        private List<Func<TVisual, TValue, TValue, Boolean>> GetChangingDelegate(TVisual forVisual)
        {
            EnsureKnown(forVisual);
            return new List<Func<TVisual, TValue, TValue, Boolean>>();
        }

        private IEnumerable<Action<TVisual, TValue, TValue>> GetOnChanged(TVisual forVisual,
                                                                          Action<TVisual, TValue, TValue>? prepend)
        {
            if (prepend != null)
                yield return prepend;

            lock (_delegateLock)
            {
                foreach (var item in _staticChangeds)
                    yield return item;
            }

            if (!_changeds.TryGetValue(forVisual, out var interested))
                yield break;


            List<Action<TVisual, TValue, TValue>> actions;
            lock (_delegateLock)
            {
                if (interested.Count == 0)
                    yield break;


                actions = new List<Action<TVisual, TValue, TValue>>(interested);
            }

            foreach (var a in actions)
                yield return a;
        }


        private IEnumerable<Func<TVisual, TValue, TValue, Boolean>> GetOnChanging(TVisual forVisual,
            Func<TVisual, TValue, TValue, Boolean>? prepend)
        {
            if (prepend != null)
                yield return prepend;

            if (_changings.TryGetValue(forVisual, out var interestedParties))
            {
                List<Func<TVisual, TValue, TValue, Boolean>> funcs;
                lock (_delegateLock)
                {
                    if (interestedParties.Count == 0)
                        yield break;

                    funcs = new List<Func<TVisual, TValue, TValue, Boolean>>(interestedParties);
                }

                foreach (var f in funcs)
                    yield return f;
            }
        }

        private void OnVisualDisposed(IVisualElement visual)
        {
            visual.Disposed -= OnVisualDisposed;
            
            if (!(visual is TVisual valid))
                return;

            _knownVisuals.TryRemove(valid, out _);

            _values.TryRemove(valid, out _);
            _computedValues.TryRemove(valid, out _);
            _changeds.TryRemove(valid, out _);
            _changings.TryRemove(valid, out _);
        }

        private void SetValueImpl(TVisual forVisual,
                                  TValue value,
                                  IEnumerable<Func<TVisual, TValue, TValue, Boolean>> onChangers,
                                  IEnumerable<Action<TVisual, TValue, TValue>> onChangeds)
        {
            if (_values.TryGetValue(forVisual, out var was))
            {
                if (Equals(was, value))
                    return;
            }
            else
            {
                //was unknown, setting it to default - no point
                if (Equals(value, _defaultValue))
                    return;

                was = _defaultValue;
            }

            foreach (var oc in onChangers)
                if (!oc(forVisual, was, value))
                    return;

            _values[forVisual] = value;

            foreach (var oc in onChangeds) oc(forVisual, was, value);

            forVisual.RaisePropertyChanged(_propertyName, value);

            //if (!_changeds.TryGetValue(forVisual, out var interested))
            //    return;


            //List<Action<TVisual, TValue, TValue>> actions;
            //lock (_delegateLock)
            //{
            //    actions = new List<Action<TVisual, TValue, TValue>>(interested);
            //}

            //foreach (var func in actions) func(forVisual, was, value);
        }

        private static Action<TVisual, TValue, TValue> WrapOnChanged(TVisual forVisual,
                                                                     Action<TValue, TValue> onChanged)
        {
            void Wrapped(TVisual _, TValue oldValue, TValue newValue)
            {
                onChanged(oldValue, newValue);
            }

            return Wrapped;
        }

        private static Func<TVisual, TValue, TValue, Boolean> WrapOnChanging(TVisual forVisual,
                                                                             Func<TValue, TValue, Boolean> onChanging)
        {
            Boolean Wrapped(TVisual _, TValue oldValue, TValue newValue)
            {
                return onChanging(oldValue, newValue);
            }

            return Wrapped;
        }

        private readonly ConcurrentDictionary<TVisual, List<Action<TVisual, TValue, TValue>>> _changeds;
        private readonly ConcurrentDictionary<TVisual, List<Func<TVisual, TValue, TValue, Boolean>>> _changings;
        private readonly TValue _defaultValue;
        private readonly Object _delegateLock;
        private readonly ConcurrentDictionary<TVisual, Byte> _knownVisuals;
        private readonly String _propertyName;
        private readonly List<Action<TVisual, TValue, TValue>> _staticChangeds;
        private readonly ConcurrentDictionary<TVisual, TValue> _values;
        
        private readonly ConcurrentDictionary<TVisual, Func<IVisualElement, Object?>> _computedValues;
    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Transitions;

namespace Das.Views.DependencyProperties
{
    public abstract class DependencyPropertyBase<TOwner, TValue> : DependencyProperty,
                                                                   IDependencyProperty<TValue>
    {
        protected DependencyPropertyBase(String propertyName,
                                       TValue defaultValue,
                                       PropertyMetadata metaData)
        {
            _delegateLock = new Object();
            Name = propertyName;
            _defaultValue = defaultValue;
            _metaData = metaData;

            _values = new ConcurrentDictionary<TOwner, TValue>();
            _computedValues = new ConcurrentDictionary<TOwner, Func<IVisualElement, Object?>>();

            _changings = new ConcurrentDictionary<TOwner, List<Func<TOwner, TValue, TValue, Boolean>>>();
            _changeds = new ConcurrentDictionary<TOwner, HashSet<Action<TOwner, TValue, TValue>>>();
            _knownVisuals = new ConcurrentDictionary<TOwner, Byte>();
            _staticChangeds = new List<Action<TOwner, TValue, TValue>>();
            _transitions = new ConcurrentDictionary<TOwner, ITransition<TValue>>();
            _visualsSetByNotStyle = new ConcurrentDictionary<TOwner, Boolean>();

            unchecked
            {
                _hashCode = typeof(TOwner).GetHashCode() +
                            (String.Intern(propertyName).GetHashCode() << 16);
            }
        }

        public override Object? GetValue(IVisualElement visual)
        {
            var tv = GetValue<IVisualElement, TOwner>(visual);
            return GetValue(tv);
        }

        public TValue GetValue<TVisual1>(TVisual1 visual) where TVisual1 : IVisualElement
        {
            var tv = GetValue<TVisual1, TOwner>(visual);
            return GetValue(tv);
        }

        //public TValue GetValue(IVisualElement visual)
        //{
        //    var tv = GetValue<IVisualElement, TVisual>(visual);
        //    return GetValue(tv);
        //}

        public override void SetValueNoTransitions(IVisualElement visual,
                                                       Object? value)
        {
            SetRuntimeValueImpl(visual, value, false, true);
        }

        public override void SetValue(IVisualElement visual,
                                          Object? value)
        {
            SetRuntimeValueImpl(visual, value, false, false);
        }

        public override void SetValueFromStyle(IVisualElement visual,
                                                   Object? value)
        {
            SetRuntimeValueImpl(visual, value, true, false);
        }

        

        public override void SetComputedValueFromStyle(IVisualElement visual,
                                                           Func<IVisualElement, Object?> value)
        {
            var tv = GetValue<IVisualElement, TOwner>(visual);
            _values.TryRemove(tv, out _);

            _computedValues[tv] = value;
            EnsureKnown(tv);
        }

        public override void AddOnChangedHandler(IVisualElement visual,
                                                     Action<IDependencyProperty> onChange)
        {
            var tVisual = GetValue<IVisualElement, TOwner>(visual);

            AddOnChangedHandler(tVisual, (_,
                                          _,
                                          _) => onChange(this));
        }

        public override void AddTransition(IVisualElement visual,
                                               ITransition transition)
        {
            var tv = GetValue<IVisualElement, TOwner>(visual);
            var ttransition = GetValue<ITransition, ITransition<TValue>>(transition);
            AddTransition(tv, ttransition);
        }

        public override Object? DefaultValue => _defaultValue;

        TValue IDependencyProperty<TValue>.DefaultValue => _defaultValue;

        //Object? IDependencyProperty.DefaultValue => DefaultValue;

        public override Type PropertyType => typeof(TValue);

        public override Type VisualType => typeof(TOwner);

        public override Boolean Equals(IDependencyProperty other)
        {
            return other is DependencyPropertyBase<TOwner, TValue> valid &&
                   String.Equals(valid.Name, Name);
        }


        public override String Name { get; }


        public void AddOnChangedHandler(Action<TOwner, TValue, TValue> handler)
        {
            lock (_delegateLock)
            {
                _staticChangeds.Add(handler);
            }
        }

        public void AddOnChangedHandler(TOwner forOwner,
                                        Action<TOwner, TValue, TValue> handler)
        {
            var items = _changeds.GetOrAdd(forOwner, GetChangedDelegate);
            lock (_delegateLock)
            {
                items.Add(handler);
            }
        }

        public void AddOnChangingHandler(TOwner forOwner,
                                         Func<TOwner, TValue, TValue, Boolean> handler)
        {
            var items = _changings.GetOrAdd(forOwner, GetChangingDelegate);
            lock (_delegateLock)
            {
                items.Add(handler);
            }
        }

        public void AddTransition(TOwner forOwner,
                                  ITransition<TValue> transition)
        {
            _transitions[forOwner] = transition;
        }

        public TValue GetValue(TOwner forOwner)
        {
            if (!TryGetKnownOrComputedValue(forOwner, out var good))
            {
                good = _defaultValue;
            }

            return good;
        }

        //public TValue GetValue(TVisual forOwner,
        //                       IStyleProvider contextStyle,
        //                       Func<TVisual, IStyleProvider, TValue> getDefault)
        //{
        //    if (!TryGetKnownOrComputedValue(forOwner, out var good))
        //    {
        //        good = getDefault(forOwner, contextStyle);
        //        EnsureKnown(forOwner);
        //    }

        //    return good;
        //}

        public void SetValue(TOwner forOwner,
                             TValue value,
                             Func<TValue, TValue, Boolean> onChanging,
                             Action<TValue, TValue> onChanged)
        {
            SetValueImpl(forOwner, value,
                GetOnChanging(forOwner, WrapOnChanging(onChanging)),
                GetOnChanged(forOwner, WrapOnChanged(onChanged)), false, false);
        }


        public void SetValue(TOwner forOwner,
                             TValue value,
                             Func<TOwner, TValue, TValue, Boolean> onChanging,
                             Action<TOwner, TValue, TValue> onChanged)
        {
            SetValueImpl(forOwner, value,
                GetOnChanging(forOwner, onChanging),
                GetOnChanged(forOwner, onChanged), false, false);
        }


        public void SetValue(TOwner forOwner,
                             TValue value)
        {
            SetValueImpl(forOwner, value, false, false);
        }

        public void SetValueNoTransitions(TOwner forOwner,
                                          TValue value)
        {
            SetValueImpl(forOwner, value, true, false);
        }

        //public override Boolean Equals(Object other)
        //{
        //   return other is DependencyProperty<TVisual, TValue> sameType &&
        //          String.Equals(sameType.Name, Name);
        //}

        public override Int32 GetHashCode()
        {
            return _hashCode;
        }

        //public static DependencyProperty<TVisual, TValue> Register(String propertyName,
        //                                                           TValue defaultValue,
        //                                                           Action<TVisual, TValue, TValue> onChanged)
        //{
        //   var res = Register(propertyName, defaultValue);
        //   res.AddOnChangedHandler(onChanged);
        //   return res;
        //}

        //public static DependencyProperty<TVisual, TValue> Register(String propertyName,
        //                                                           TValue defaultValue,
        //                                                           PropertyMetadata metadata)
        //{
        //   var dep = new DependencyProperty<TVisual, TValue>(propertyName, defaultValue, metadata);

        //   DependencyProperty.NotifyTypeRegistration(dep);

        //   return dep;
        //}


        //public static DependencyProperty<TVisual, TValue> Register(String propertyName,
        //                                                           TValue defaultValue)
        //{
        //   return Register(propertyName, defaultValue, PropertyMetadata.None);
        //}

        public void SetValue(TOwner forOwner,
                             TValue value,
                             Action<TValue, TValue> onChanged)
        {
            SetValueImpl(forOwner, value,
                GetOnChanging(forOwner, null),
                GetOnChanged(forOwner, WrapOnChanged(onChanged)),
                false, false);
        }

        public override String ToString()
        {
            return Name;
        }

        private void EnsureKnown(TOwner forOwner)
        {
            if (forOwner is INotifyDisposable disposable)
            {
                if (!disposable.IsDisposed && !_knownVisuals.TryGetValue(forOwner, out _))
                {
                    disposable.Disposed += OnVisualDisposed;
                    _knownVisuals[forOwner] = 0;
                }
            }
            else if (!_knownVisuals.TryGetValue(forOwner, out _))
            {
                _knownVisuals[forOwner] = 0;
            }
        }

        private HashSet<Action<TOwner, TValue, TValue>> GetChangedDelegate(TOwner forOwner)
        {
            EnsureKnown(forOwner);
            return new HashSet<Action<TOwner, TValue, TValue>>();
        }

        private List<Func<TOwner, TValue, TValue, Boolean>> GetChangingDelegate(TOwner forOwner)
        {
            EnsureKnown(forOwner);
            return new List<Func<TOwner, TValue, TValue, Boolean>>();
        }

        private TValue GetOldValue(TOwner forOwner)
        {
            if (_values.TryGetValue(forOwner, out var was))
                return was;

            return _defaultValue;
        }

        private IEnumerable<Action<TOwner, TValue, TValue>> GetOnChanged(TOwner forOwner,
                                                                          Action<TOwner, TValue, TValue>? prepend)
        {
            if (prepend != null)
                yield return prepend;

            lock (_delegateLock)
            {
                foreach (var item in _staticChangeds)
                {
                    yield return item;
                }
            }

            if (!_changeds.TryGetValue(forOwner, out var interested))
                yield break;


            List<Action<TOwner, TValue, TValue>> actions;
            lock (_delegateLock)
            {
                if (interested.Count == 0)
                    yield break;


                actions = new List<Action<TOwner, TValue, TValue>>(interested);
            }

            foreach (var a in actions)
            {
                yield return a;
            }
        }


        private IEnumerable<Func<TOwner, TValue, TValue, Boolean>> GetOnChanging(TOwner forOwner,
            Func<TOwner, TValue, TValue, Boolean>? prepend)
        {
            if (prepend != null)
                yield return prepend;

            if (_changings.TryGetValue(forOwner, out var interestedParties))
            {
                List<Func<TOwner, TValue, TValue, Boolean>> funcs;
                lock (_delegateLock)
                {
                    if (interestedParties.Count == 0)
                        yield break;

                    funcs = new List<Func<TOwner, TValue, TValue, Boolean>>(interestedParties);
                }

                foreach (var f in funcs)
                {
                    yield return f;
                }
            }
        }


        private static TOut GetValue<TIn, TOut>(TIn value)
        {
            if (Equals(value, default))
                return default!;

            if (value is not TOut tv)
                throw new InvalidCastException(value + " cannot be cast to type " + typeof(TOut));

            return tv;
        }

        private Boolean HasMetadataFlag(PropertyMetadata flag)
        {
            return (_metaData & flag) == flag;
        }

        public override void ClearValue(IVisualElement visual)
        {
            visual.Disposed -= OnVisualDisposed;

            if (!(visual is TOwner valid))
                return;

            _values.TryRemove(valid, out _);
            _computedValues.TryRemove(valid, out _);

            _changings.TryRemove(valid, out _);
            _changeds.TryRemove(valid, out _);

            _knownVisuals.TryRemove(valid, out _);

            _transitions.TryRemove(valid, out _);

            _visualsSetByNotStyle.TryRemove(valid, out _);
        }

        private void OnVisualDisposed(IVisualElement visual)
        {
           ClearValue(visual);
        }

        private void SetRuntimeValueImpl(IVisualElement visual,
                                         Object? value,
                                         Boolean isFromStyle,
                                         Boolean isDeclineTransitions)
        {
            var tv = GetValue<IVisualElement, TOwner>(visual);
            var tValue = GetValue<Object?, TValue>(value);
            SetValueImpl(tv, tValue, isFromStyle, isDeclineTransitions);
        }

        private void SetValueImpl(TOwner forOwner,
                                  TValue value,
                                  Boolean isFromStyle,
                                  Boolean isDeclineTransitions)
        {
            SetValueImpl(forOwner, value,
                GetOnChanging(forOwner, null),
                GetOnChanged(forOwner, null),
                isFromStyle, isDeclineTransitions);
        }

        /// <summary>
        ///     All setters should pass through here...
        /// </summary>
        private void SetValueImpl(TOwner forOwner,
                                  TValue value,
                                  IEnumerable<Func<TOwner, TValue, TValue, Boolean>> onChangers,
                                  IEnumerable<Action<TOwner, TValue, TValue>> onChangeds,
                                  Boolean isFromStyle,
                                  Boolean isDeclineTransitions)
        {
            var oldValue = GetOldValue(forOwner);

            if (Equals(value, oldValue))
                return; //trying to set it to the same value - no point  

            if (!isDeclineTransitions)
                // let subscribers veto this change if desired but not if we're updating as part of 
                // a transition
                foreach (var oc in onChangers)
                {
                    if (!oc(forOwner, oldValue, value))
                        return;
                }

            if (!isDeclineTransitions)
            {
                switch (isFromStyle)
                {
                    case true when _visualsSetByNotStyle.TryGetValue(forOwner,
                        out var well) && well:
                        return; // attempt to set a value via a style when that is no longer possible

                    case false:
                        _visualsSetByNotStyle[forOwner] = true;
                        break;
                }

                if (_transitions.TryGetValue(forOwner, out var transition))
                {
                    transition.SetValue(oldValue, value);
                    return;
                }
            }

            var isDisposed = forOwner is INotifyDisposable { IsDisposed: true };

            if (Equals(value, DefaultValue) || isDisposed)
            {
                _values.TryRemove(forOwner, out _);
            }
            else
            {
                EnsureKnown(forOwner);
                _values[forOwner] = value;
            }

            if (isDisposed)
                return;

            foreach (var oc in onChangeds)
            {
                oc(forOwner, oldValue, value);
            }

            if (forOwner is IVisualElement visual)
            {
                visual.RaisePropertyChanged(Name, value);

                if (HasMetadataFlag(PropertyMetadata.AffectsMeasure))
                    visual.InvalidateMeasure();

                else if (HasMetadataFlag(PropertyMetadata.AffectsArrange))
                    visual.InvalidateArrange();
            }

            //forOwner.RaisePropertyChanged(Name, value);

            //if (HasMetadataFlag(PropertyMetadata.AffectsMeasure))
            //   forOwner.InvalidateMeasure();

            //else if (HasMetadataFlag(PropertyMetadata.AffectsArrange))
            //   forOwner.InvalidateArrange();
        }

        private Boolean TryGetKnownOrComputedValue(TOwner forOwner,
                                                   out TValue value)
        {
            if (_values.TryGetValue(forOwner, out value))
                return true;

            if (forOwner is IVisualElement visual &&
                _computedValues.TryGetValue(forOwner, out var computer))
            {
                var computed = computer(visual);
                value = GetValue<Object, TValue>(computed!);
                return true;
            }

            value = default!;
            return false;
        }

        private static Action<TOwner, TValue, TValue> WrapOnChanged(Action<TValue, TValue> onChanged)
        {
            void Wrapped(TOwner _,
                         TValue oldValue,
                         TValue newValue)
            {
                onChanged(oldValue, newValue);
            }

            return Wrapped;
        }

        private static Func<TOwner, TValue, TValue, Boolean> WrapOnChanging(Func<TValue, TValue, Boolean> onChanging)
        {
            Boolean Wrapped(TOwner _,
                            TValue oldValue,
                            TValue newValue)
            {
                return onChanging(oldValue, newValue);
            }

            return Wrapped;
        }

        private readonly ConcurrentDictionary<TOwner, HashSet<Action<TOwner, TValue, TValue>>> _changeds;
        private readonly ConcurrentDictionary<TOwner, List<Func<TOwner, TValue, TValue, Boolean>>> _changings;

        private readonly ConcurrentDictionary<TOwner, Func<IVisualElement, Object?>> _computedValues;
        private readonly Object _delegateLock;

        private readonly Int32 _hashCode;
        private readonly ConcurrentDictionary<TOwner, Byte> _knownVisuals;
        private readonly PropertyMetadata _metaData;
        private readonly List<Action<TOwner, TValue, TValue>> _staticChangeds;
        private readonly ConcurrentDictionary<TOwner, ITransition<TValue>> _transitions;
        private readonly ConcurrentDictionary<TOwner, TValue> _values;

        /// <summary>
        ///     Once a property's value has been set by something other than a style (binding, direct set)
        ///     it isn't eligible to be set by a style anymore
        /// </summary>
        private readonly ConcurrentDictionary<TOwner, Boolean> _visualsSetByNotStyle;

        private readonly TValue _defaultValue;
    }
}

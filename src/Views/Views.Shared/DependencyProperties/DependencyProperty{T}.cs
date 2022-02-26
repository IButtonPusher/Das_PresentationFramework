using System;
using System.Threading.Tasks;
using Das.Views.DataBinding;
using Das.Views.DependencyProperties;

namespace Das.Views
{
    public class DependencyProperty<TValue> : DependencyPropertyBase<IBindableElement, TValue>
    {
        private DependencyProperty(String propertyName,
                                   TValue defaultValue,
                                   PropertyMetadata metaData)
            : base(propertyName, defaultValue, metaData)
        {
            //_delegateLock = new Object();
            //Name = propertyName;
            //DefaultValue = defaultValue;
            //_metaData = metaData;

            //_values = new ConcurrentDictionary<IBindableElement, TValue>();
            //_computedValues = new ConcurrentDictionary<IBindableElement, Func<IVisualElement, Object?>>();

            //_changings =
            //    new ConcurrentDictionary<IBindableElement, List<Func<IBindableElement, TValue, TValue, Boolean>>>();
            //_changeds = new ConcurrentDictionary<IBindableElement, HashSet<Action<IBindableElement, TValue, TValue>>>();
            //_knownVisuals = new ConcurrentDictionary<IBindableElement, Byte>();
            //_staticChangeds = new List<Action<IBindableElement, TValue, TValue>>();
            //_transitions = new ConcurrentDictionary<IBindableElement, ITransition<TValue>>();
            //_visualsSetByNotStyle = new ConcurrentDictionary<IBindableElement, Boolean>();

            //unchecked
            //{
            //    _hashCode = typeof(IBindableElement).GetHashCode() +
            //                (String.Intern(propertyName).GetHashCode() << 16);
            //}

            //unchecked
            //{
            //    _hashCode = typeof(IBindableElement).GetHashCode();
            //    _hashCode = (_hashCode * 397) ^ typeof(TValue).GetHashCode();
            //    _hashCode = (_hashCode * 397) ^ Name.GetHashCode();
            //}
        }

        public static DependencyProperty<TValue> Register(String propertyName,
                                                          TValue defaultValue,
                                                          PropertyMetadata metadata,
                                                          Type ownerType)
        {
            var dep = new DependencyProperty<TValue>(propertyName, defaultValue, metadata);

            NotifyTypeRegistration(dep, ownerType);

            return dep;
        }


        public static DependencyProperty<TValue> Register(String propertyName,
                                                          TValue defaultValue,
                                                          Type ownerType)
        {
            return Register(propertyName, defaultValue, PropertyMetadata.None, ownerType);
        }


        //Object? IDependencyProperty.GetValue(IVisualElement visual)
        //{
        //    var tv = GetValue<IVisualElement, IBindableElement>(visual);
        //    return GetValue(tv);
        //}

        //public TValue GetValue<IBindableElement1>(IBindableElement1 visual) where IBindableElement1 : IVisualElement
        //{
        //    var tv = GetValue<IBindableElement1, IBindableElement>(visual);
        //    return GetValue(tv);
        //}

        //public TValue GetValue(IVisualElement visual)
        //{
        //    var tv = GetValue<IVisualElement, IBindableElement>(visual);
        //    return GetValue(tv);
        //}

        //void IDependencyProperty.SetValueNoTransitions(IVisualElement visual,
        //                                               Object? value)
        //{
        //    SetRuntimeValueImpl(visual, value, false, true);
        //}

        //void IDependencyProperty.SetValue(IVisualElement visual,
        //                                  Object? value)
        //{
        //    SetRuntimeValueImpl(visual, value, false, false);
        //}

        //void IDependencyProperty.SetValueFromStyle(IVisualElement visual,
        //                                           Object? value)
        //{
        //    SetRuntimeValueImpl(visual, value, true, false);
        //}


        //void IDependencyProperty.SetComputedValueFromStyle(IVisualElement visual,
        //                                                   Func<IVisualElement, Object?> value)
        //{
        //    var tv = GetValue<IVisualElement, IBindableElement>(visual);
        //    _values.TryRemove(tv, out _);

        //    _computedValues[tv] = value;
        //    EnsureKnown(tv);
        //}

        //void IDependencyProperty.AddOnChangedHandler(IVisualElement visual,
        //                                             Action<IDependencyProperty> onChange)
        //{
        //    var IBindableElement = GetValue<IVisualElement, IBindableElement>(visual);

        //    AddOnChangedHandler(IBindableElement, (_,
        //                                           _,
        //                                           _) => onChange(this));
        //}

        //void IDependencyProperty.AddTransition(IVisualElement visual,
        //                                       ITransition transition)
        //{
        //    var tv = GetValue<IVisualElement, IBindableElement>(visual);
        //    var ttransition = GetValue<ITransition, ITransition<TValue>>(transition);
        //    AddTransition(tv, ttransition);
        //}

        //public TValue DefaultValue { get; }

        //Object? IDependencyProperty.DefaultValue => DefaultValue;

        //public Type PropertyType => typeof(TValue);

        //public Type VisualType => typeof(IBindableElement);

        //public Boolean Equals(IDependencyProperty other)
        //{
        //    return other is DependencyProperty<IBindableElement, TValue> valid &&
        //           String.Equals(valid.Name, Name);
        //}


        //public String Name { get; }


        //public void AddOnChangedHandler(Action<IBindableElement, TValue, TValue> handler)
        //{
        //    lock (_delegateLock)
        //    {
        //        _staticChangeds.Add(handler);
        //    }
        //}

        //public void AddOnChangedHandler(IBindableElement forVisual,
        //                                Action<IBindableElement, TValue, TValue> handler)
        //{
        //    var items = _changeds.GetOrAdd(forVisual, GetChangedDelegate);
        //    lock (_delegateLock)
        //    {
        //        items.Add(handler);
        //    }
        //}

        //public void AddOnChangingHandler(IBindableElement forVisual,
        //                                 Func<IBindableElement, TValue, TValue, Boolean> handler)
        //{
        //    var items = _changings.GetOrAdd(forVisual, GetChangingDelegate);
        //    lock (_delegateLock)
        //    {
        //        items.Add(handler);
        //    }
        //}

        //public void AddTransition(IBindableElement forVisual,
        //                          ITransition<TValue> transition)
        //{
        //    _transitions[forVisual] = transition;
        //}

        //public TValue GetValue(IBindableElement forVisual)
        //{
        //    if (!TryGetKnownOrComputedValue(forVisual, out var good))
        //    {
        //        good = DefaultValue;
        //        //EnsureKnown(forVisual);
        //    }

        //    return good;
        //}

        //public TValue GetValue(IBindableElement forVisual,
        //                       IStyleProvider contextStyle,
        //                       Func<IBindableElement, IStyleProvider, TValue> getDefault)
        //{
        //    if (!TryGetKnownOrComputedValue(forVisual, out var good))
        //    {
        //        good = getDefault(forVisual, contextStyle);
        //        EnsureKnown(forVisual);
        //    }

        //    return good;
        //}

        //public void SetValue(IBindableElement forVisual,
        //                     TValue value,
        //                     Func<TValue, TValue, Boolean> onChanging,
        //                     Action<TValue, TValue> onChanged)
        //{
        //    SetValueImpl(forVisual, value,
        //        GetOnChanging(forVisual, WrapOnChanging(onChanging)),
        //        GetOnChanged(forVisual, WrapOnChanged(onChanged)), false, false);
        //}


        //public void SetValue(IBindableElement forVisual,
        //                     TValue value,
        //                     Func<IBindableElement, TValue, TValue, Boolean> onChanging,
        //                     Action<IBindableElement, TValue, TValue> onChanged)
        //{
        //    SetValueImpl(forVisual, value,
        //        GetOnChanging(forVisual, onChanging),
        //        GetOnChanged(forVisual, onChanged), false, false);
        //}


        //public void SetValue(IBindableElement forVisual,
        //                     TValue value)
        //{
        //    SetValueImpl(forVisual, value, false, false);
        //}

        //public void SetValueNoTransitions(IBindableElement forVisual,
        //                                  TValue value)
        //{
        //    SetValueImpl(forVisual, value, true, false);
        //}

        //public override Boolean Equals(Object other)
        //{
        //    return other is DependencyProperty<IBindableElement, TValue> sameType &&
        //           String.Equals(sameType.Name, Name);
        //}

        //public override Int32 GetHashCode()
        //{
        //    return _hashCode;
        //}

        //public static DependencyProperty<IBindableElement, TValue> Register(String propertyName,
        //                                                           TValue defaultValue,
        //                                                           Action<IBindableElement, TValue, TValue> onChanged)
        //{
        //   var res = Register(propertyName, defaultValue);
        //   res.AddOnChangedHandler(onChanged);
        //   return res;
        //}


        //public void SetValue(IBindableElement forVisual,
        //                     TValue value,
        //                     Action<TValue, TValue> onChanged)
        //{
        //    SetValueImpl(forVisual, value,
        //        GetOnChanging(forVisual, null),
        //        GetOnChanged(forVisual, WrapOnChanged(onChanged)),
        //        false, false);
        //}

        //public override String ToString()
        //{
        //    return Name;
        //}

        //private void EnsureKnown(IBindableElement forVisual)
        //{
        //    if (!forVisual.IsDisposed && !_knownVisuals.TryGetValue(forVisual, out _))
        //    {
        //        forVisual.Disposed += OnVisualDisposed;
        //        _knownVisuals[forVisual] = 0;
        //    }
        //}

        //private HashSet<Action<IBindableElement, TValue, TValue>> GetChangedDelegate(IBindableElement forVisual)
        //{
        //    EnsureKnown(forVisual);
        //    return new HashSet<Action<IBindableElement, TValue, TValue>>();
        //}

        //private List<Func<IBindableElement, TValue, TValue, Boolean>> GetChangingDelegate(IBindableElement forVisual)
        //{
        //    EnsureKnown(forVisual);
        //    return new List<Func<IBindableElement, TValue, TValue, Boolean>>();
        //}

        //private TValue GetOldValue(IBindableElement forVisual)
        //{
        //    if (_values.TryGetValue(forVisual, out var was))
        //        return was;

        //    return DefaultValue;
        //}

        //private IEnumerable<Action<IBindableElement, TValue, TValue>> GetOnChanged(IBindableElement forVisual,
        //    Action<IBindableElement, TValue, TValue>? prepend)
        //{
        //    if (prepend != null)
        //        yield return prepend;

        //    lock (_delegateLock)
        //    {
        //        foreach (var item in _staticChangeds)
        //        {
        //            yield return item;
        //        }
        //    }

        //    if (!_changeds.TryGetValue(forVisual, out var interested))
        //        yield break;


        //    List<Action<IBindableElement, TValue, TValue>> actions;
        //    lock (_delegateLock)
        //    {
        //        if (interested.Count == 0)
        //            yield break;


        //        actions = new List<Action<IBindableElement, TValue, TValue>>(interested);
        //    }

        //    foreach (var a in actions)
        //    {
        //        yield return a;
        //    }
        //}


        //private IEnumerable<Func<IBindableElement, TValue, TValue, Boolean>> GetOnChanging(IBindableElement forVisual,
        //    Func<IBindableElement, TValue, TValue, Boolean>? prepend)
        //{
        //    if (prepend != null)
        //        yield return prepend;

        //    if (_changings.TryGetValue(forVisual, out var interestedParties))
        //    {
        //        List<Func<IBindableElement, TValue, TValue, Boolean>> funcs;
        //        lock (_delegateLock)
        //        {
        //            if (interestedParties.Count == 0)
        //                yield break;

        //            funcs = new List<Func<IBindableElement, TValue, TValue, Boolean>>(interestedParties);
        //        }

        //        foreach (var f in funcs)
        //        {
        //            yield return f;
        //        }
        //    }
        //}


        //private static TOut GetValue<TIn, TOut>(TIn value)
        //{
        //    if (Equals(value, default))
        //        return default!;

        //    if (value is not TOut tv)
        //        throw new InvalidCastException(value + " cannot be cast to type " + typeof(TOut));

        //    return tv;
        //}

        //private Boolean HasMetadataFlag(PropertyMetadata flag)
        //{
        //    return (_metaData & flag) == flag;
        //}

        //public void ClearValue(IVisualElement visual)
        //{
        //    visual.Disposed -= OnVisualDisposed;

        //    if (!(visual is IBindableElement valid))
        //        return;

        //    _values.TryRemove(valid, out _);
        //    _computedValues.TryRemove(valid, out _);

        //    _changings.TryRemove(valid, out _);
        //    _changeds.TryRemove(valid, out _);

        //    _knownVisuals.TryRemove(valid, out _);

        //    _transitions.TryRemove(valid, out _);

        //    _visualsSetByNotStyle.TryRemove(valid, out _);
        //}

        //private void OnVisualDisposed(IVisualElement visual)
        //{
        //   ClearValue(visual);
        //}

        //private void SetRuntimeValueImpl(IVisualElement visual,
        //                                 Object? value,
        //                                 Boolean isFromStyle,
        //                                 Boolean isDeclineTransitions)
        //{
        //    var tv = GetValue<IVisualElement, IBindableElement>(visual);
        //    var tValue = GetValue<Object?, TValue>(value);
        //    SetValueImpl(tv, tValue, isFromStyle, isDeclineTransitions);
        //}

        //private void SetValueImpl(IBindableElement forVisual,
        //                          TValue value,
        //                          Boolean isFromStyle,
        //                          Boolean isDeclineTransitions)
        //{
        //    SetValueImpl(forVisual, value,
        //        GetOnChanging(forVisual, null),
        //        GetOnChanged(forVisual, null),
        //        isFromStyle, isDeclineTransitions);
        //}

        ///// <summary>
        /////     All setters should pass through here...
        ///// </summary>
        //private void SetValueImpl(IBindableElement forVisual,
        //                          TValue value,
        //                          IEnumerable<Func<IBindableElement, TValue, TValue, Boolean>> onChangers,
        //                          IEnumerable<Action<IBindableElement, TValue, TValue>> onChangeds,
        //                          Boolean isFromStyle,
        //                          Boolean isDeclineTransitions)
        //{
        //    var oldValue = GetOldValue(forVisual);

        //    if (Equals(value, oldValue))
        //        return; //trying to set it to the same value - no point  

        //    if (!isDeclineTransitions)
        //        // let subscribers veto this change if desired but not if we're updating as part of 
        //        // a transition
        //        foreach (var oc in onChangers)
        //        {
        //            if (!oc(forVisual, oldValue, value))
        //                return;
        //        }

        //    if (!isDeclineTransitions)
        //    {
        //        switch (isFromStyle)
        //        {
        //            case true when _visualsSetByNotStyle.TryGetValue(forVisual,
        //                out var well) && well:
        //                return; // attempt to set a value via a style when that is no longer possible

        //            case false:
        //                _visualsSetByNotStyle[forVisual] = true;
        //                break;
        //        }

        //        if (_transitions.TryGetValue(forVisual, out var transition))
        //        {
        //            transition.SetValue(oldValue, value);
        //            return;
        //        }
        //    }

        //    if (Equals(value, DefaultValue) || forVisual.IsDisposed)
        //    {
        //        _values.TryRemove(forVisual, out _);
        //    }
        //    else
        //    {
        //        EnsureKnown(forVisual);
        //        _values[forVisual] = value;
        //    }

        //    if (forVisual.IsDisposed)
        //        return;

        //    foreach (var oc in onChangeds)
        //    {
        //        oc(forVisual, oldValue, value);
        //    }

        //    forVisual.RaisePropertyChanged(Name, value);

        //    if (HasMetadataFlag(PropertyMetadata.AffectsMeasure))
        //        forVisual.InvalidateMeasure();

        //    else if (HasMetadataFlag(PropertyMetadata.AffectsArrange))
        //        forVisual.InvalidateArrange();
        //}

        //private Boolean TryGetKnownOrComputedValue(IBindableElement forVisual,
        //                                           out TValue value)
        //{
        //    if (_values.TryGetValue(forVisual, out value))
        //        return true;

        //    if (_computedValues.TryGetValue(forVisual, out var computer))
        //    {
        //        var computed = computer(forVisual);
        //        value = GetValue<Object, TValue>(computed!);
        //        return true;
        //    }

        //    value = default!;
        //    return false;
        //}

        //private static Action<IBindableElement, TValue, TValue> WrapOnChanged(Action<TValue, TValue> onChanged)
        //{
        //    void Wrapped(IBindableElement _,
        //                 TValue oldValue,
        //                 TValue newValue)
        //    {
        //        onChanged(oldValue, newValue);
        //    }

        //    return Wrapped;
        //}

        //private static Func<IBindableElement, TValue, TValue, Boolean> WrapOnChanging(
        //    Func<TValue, TValue, Boolean> onChanging)
        //{
        //    Boolean Wrapped(IBindableElement _,
        //                    TValue oldValue,
        //                    TValue newValue)
        //    {
        //        return onChanging(oldValue, newValue);
        //    }

        //    return Wrapped;
        //}

        //private readonly ConcurrentDictionary<IBindableElement, HashSet<Action<IBindableElement, TValue, TValue>>>
        //    _changeds;

        //private readonly ConcurrentDictionary<IBindableElement, List<Func<IBindableElement, TValue, TValue, Boolean>>>
        //    _changings;

        //private readonly ConcurrentDictionary<IBindableElement, Func<IVisualElement, Object?>> _computedValues;
        //private readonly Object _delegateLock;

        //private readonly Int32 _hashCode;
        //private readonly ConcurrentDictionary<IBindableElement, Byte> _knownVisuals;
        //private readonly PropertyMetadata _metaData;
        //private readonly List<Action<IBindableElement, TValue, TValue>> _staticChangeds;
        //private readonly ConcurrentDictionary<IBindableElement, ITransition<TValue>> _transitions;
        //private readonly ConcurrentDictionary<IBindableElement, TValue> _values;

        /// <summary>
        ///     Once a property's value has been set by something other than a style (binding, direct set)
        ///     it isn't eligible to be set by a style anymore
        /// </summary>
        //private readonly ConcurrentDictionary<IBindableElement, Boolean> _visualsSetByNotStyle;
    }
}

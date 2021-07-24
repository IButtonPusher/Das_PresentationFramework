using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.DependencyProperties;
using Das.Views.Transitions;

namespace Das.Views
{
   public class DependencyProperty<TVisual, TValue> : IDependencyProperty<TVisual, TValue>,
                                                      IDependencyProperty<TValue>
      where TVisual : IVisualElement
   {
      private DependencyProperty(String propertyName,
                                 TValue defaultValue,
                                 PropertyMetadata metaData)
      {
         _delegateLock = new Object();
         Name = propertyName;
         DefaultValue = defaultValue;
         _metaData = metaData;

         _values = new ConcurrentDictionary<TVisual, TValue>();
         _computedValues = new ConcurrentDictionary<TVisual, Func<IVisualElement, Object?>>();

         _changings = new ConcurrentDictionary<TVisual, List<Func<TVisual, TValue, TValue, Boolean>>>();
         _changeds = new ConcurrentDictionary<TVisual, HashSet<Action<TVisual, TValue, TValue>>>();
         _knownVisuals = new ConcurrentDictionary<TVisual, Byte>();
         _staticChangeds = new List<Action<TVisual, TValue, TValue>>();
         _transitions = new ConcurrentDictionary<TVisual, ITransition<TValue>>();
         _visualsSetByNotStyle = new ConcurrentDictionary<TVisual, Boolean>();

         unchecked
         {
            _hashCode = typeof(TVisual).GetHashCode() +
                        (String.Intern(propertyName).GetHashCode() << 16);
         }

         //unchecked
         //{
         //    _hashCode = typeof(TVisual).GetHashCode();
         //    _hashCode = (_hashCode * 397) ^ typeof(TValue).GetHashCode();
         //    _hashCode = (_hashCode * 397) ^ Name.GetHashCode();
         //}
      }

      Object? IDependencyProperty.GetValue(IVisualElement visual)
      {
         var tv = GetValue<IVisualElement, TVisual>(visual);
         return GetValue(tv);
      }

      public TValue GetValue<TVisual1>(TVisual1 visual) where TVisual1 : IVisualElement
      {
         var tv = GetValue<TVisual1, TVisual>(visual);
         return GetValue(tv);
      }

      //public TValue GetValue(IVisualElement visual)
      //{
      //    var tv = GetValue<IVisualElement, TVisual>(visual);
      //    return GetValue(tv);
      //}

      void IDependencyProperty.SetValueNoTransitions(IVisualElement visual,
                                                     Object? value)
      {
         SetRuntimeValueImpl(visual, value, false, true);
      }

      void IDependencyProperty.SetValue(IVisualElement visual,
                                        Object? value)
      {
         SetRuntimeValueImpl(visual, value, false, false);
      }

      void IDependencyProperty.SetValueFromStyle(IVisualElement visual,
                                                 Object? value)
      {
         SetRuntimeValueImpl(visual, value, true, false);
      }

      void IDependencyProperty.SetComputedValueFromStyle(IVisualElement visual,
                                                         Func<IVisualElement, Object?> value)
      {
         var tv = GetValue<IVisualElement, TVisual>(visual);
         _values.TryRemove(tv, out _);

         _computedValues[tv] = value;
         EnsureKnown(tv);
      }

      void IDependencyProperty.AddOnChangedHandler(IVisualElement visual,
                                                   Action<IDependencyProperty> onChange)
      {
         var tVisual = GetValue<IVisualElement, TVisual>(visual);

         AddOnChangedHandler(tVisual, (_,
                                       _,
                                       _) => onChange(this));
      }

      void IDependencyProperty.AddTransition(IVisualElement visual,
                                             ITransition transition)
      {
         var tv = GetValue<IVisualElement, TVisual>(visual);
         var ttransition = GetValue<ITransition, ITransition<TValue>>(transition);
         AddTransition(tv, ttransition);
      }

      public TValue DefaultValue { get; }

      Object? IDependencyProperty.DefaultValue => DefaultValue;

      public Type PropertyType => typeof(TValue);

      public Type VisualType => typeof(TVisual);

      public Boolean Equals(IDependencyProperty other)
      {
         return other is DependencyProperty<TVisual, TValue> valid &&
                String.Equals(valid.Name, Name);
      }


      public String Name { get; }


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

      public void AddTransition(TVisual forVisual,
                                ITransition<TValue> transition)
      {
         _transitions[forVisual] = transition;
      }

      public TValue GetValue(TVisual forVisual)
      {
         if (!TryGetKnownOrComputedValue(forVisual, out var good))
         {
            good = DefaultValue;
            //EnsureKnown(forVisual);
         }

         return good;
      }

      //public TValue GetValue(TVisual forVisual,
      //                       IStyleProvider contextStyle,
      //                       Func<TVisual, IStyleProvider, TValue> getDefault)
      //{
      //    if (!TryGetKnownOrComputedValue(forVisual, out var good))
      //    {
      //        good = getDefault(forVisual, contextStyle);
      //        EnsureKnown(forVisual);
      //    }

      //    return good;
      //}

      public void SetValue(TVisual forVisual,
                           TValue value,
                           Func<TValue, TValue, Boolean> onChanging,
                           Action<TValue, TValue> onChanged)
      {
         SetValueImpl(forVisual, value,
            GetOnChanging(forVisual, WrapOnChanging(onChanging)),
            GetOnChanged(forVisual, WrapOnChanged(onChanged)), false, false);
      }


      public void SetValue(TVisual forVisual,
                           TValue value,
                           Func<TVisual, TValue, TValue, Boolean> onChanging,
                           Action<TVisual, TValue, TValue> onChanged)
      {
         SetValueImpl(forVisual, value,
            GetOnChanging(forVisual, onChanging),
            GetOnChanged(forVisual, onChanged), false, false);
      }


      public void SetValue(TVisual forVisual,
                           TValue value)
      {
         SetValueImpl(forVisual, value, false, false);
      }

      public void SetValueNoTransitions(TVisual forVisual,
                                        TValue value)
      {
         SetValueImpl(forVisual, value, true, false);
      }

      public override Boolean Equals(Object other)
      {
         return other is DependencyProperty<TVisual, TValue> sameType &&
                String.Equals(sameType.Name, Name);
      }

      public override Int32 GetHashCode()
      {
         return _hashCode;
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
                                                                 TValue defaultValue,
                                                                 PropertyMetadata metadata)
      {
         var dep = new DependencyProperty<TVisual, TValue>(propertyName, defaultValue, metadata);

         DependencyProperty.NotifyTypeRegistration(dep);

         return dep;
      }


      public static DependencyProperty<TVisual, TValue> Register(String propertyName,
                                                                 TValue defaultValue)
      {
         return Register(propertyName, defaultValue, PropertyMetadata.None);
      }

      public void SetValue(TVisual forVisual,
                           TValue value,
                           Action<TValue, TValue> onChanged)
      {
         SetValueImpl(forVisual, value,
            GetOnChanging(forVisual, null),
            GetOnChanged(forVisual, WrapOnChanged(onChanged)),
            false, false);
      }

      public override String ToString()
      {
         return Name;
      }

      private void EnsureKnown(TVisual forVisual)
      {
         if (!forVisual.IsDisposed && !_knownVisuals.TryGetValue(forVisual, out _))
         {
            forVisual.Disposed += OnVisualDisposed;
            _knownVisuals[forVisual] = 0;
         }
      }

      private HashSet<Action<TVisual, TValue, TValue>> GetChangedDelegate(TVisual forVisual)
      {
         EnsureKnown(forVisual);
         return new HashSet<Action<TVisual, TValue, TValue>>();
      }

      private List<Func<TVisual, TValue, TValue, Boolean>> GetChangingDelegate(TVisual forVisual)
      {
         EnsureKnown(forVisual);
         return new List<Func<TVisual, TValue, TValue, Boolean>>();
      }

      private TValue GetOldValue(TVisual forVisual)
      {
         if (_values.TryGetValue(forVisual, out var was))
            return was;

         return DefaultValue;
      }

      private IEnumerable<Action<TVisual, TValue, TValue>> GetOnChanged(TVisual forVisual,
                                                                        Action<TVisual, TValue, TValue>? prepend)
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
         {
            yield return a;
         }
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
         _transitions.TryRemove(valid, out _);
      }

      private void SetRuntimeValueImpl(IVisualElement visual,
                                       Object? value,
                                       Boolean isFromStyle,
                                       Boolean isDeclineTransitions)
      {
         var tv = GetValue<IVisualElement, TVisual>(visual);
         var tValue = GetValue<Object?, TValue>(value);
         SetValueImpl(tv, tValue, isFromStyle, isDeclineTransitions);
      }

      private void SetValueImpl(TVisual forVisual,
                                TValue value,
                                Boolean isFromStyle,
                                Boolean isDeclineTransitions)
      {
         SetValueImpl(forVisual, value,
            GetOnChanging(forVisual, null),
            GetOnChanged(forVisual, null),
            isFromStyle, isDeclineTransitions);
      }

      /// <summary>
      ///    All setters should pass through here...
      /// </summary>
      private void SetValueImpl(TVisual forVisual,
                                TValue value,
                                IEnumerable<Func<TVisual, TValue, TValue, Boolean>> onChangers,
                                IEnumerable<Action<TVisual, TValue, TValue>> onChangeds,
                                Boolean isFromStyle,
                                Boolean isDeclineTransitions)
      {
         var oldValue = GetOldValue(forVisual);

         if (Equals(value, oldValue))
            return; //trying to set it to the same value - no point  

         if (!isDeclineTransitions)
            // let subscribers veto this change if desired but not if we're updating as part of 
            // a transition
            foreach (var oc in onChangers)
            {
               if (!oc(forVisual, oldValue, value))
                  return;
            }

         if (!isDeclineTransitions)
         {
            switch (isFromStyle)
            {
               case true when _visualsSetByNotStyle.TryGetValue(forVisual,
                  out var well) && well:
                  return; // attempt to set a value via a style when that is no longer possible

               case false:
                  _visualsSetByNotStyle[forVisual] = true;
                  break;
            }


            if (_transitions.TryGetValue(forVisual, out var transition))
            {
               transition.SetValue(oldValue, value);
               return;
            }
         }

         _values[forVisual] = value;

         foreach (var oc in onChangeds)
         {
            oc(forVisual, oldValue, value);
         }

         forVisual.RaisePropertyChanged(Name, value);

         if (HasMetadataFlag(PropertyMetadata.AffectsMeasure))
            forVisual.InvalidateMeasure();

         else if (HasMetadataFlag(PropertyMetadata.AffectsArrange))
            forVisual.InvalidateArrange();
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

      private static Action<TVisual, TValue, TValue> WrapOnChanged(Action<TValue, TValue> onChanged)
      {
         void Wrapped(TVisual _,
                      TValue oldValue,
                      TValue newValue)
         {
            onChanged(oldValue, newValue);
         }

         return Wrapped;
      }

      private static Func<TVisual, TValue, TValue, Boolean> WrapOnChanging(Func<TValue, TValue, Boolean> onChanging)
      {
         Boolean Wrapped(TVisual _,
                         TValue oldValue,
                         TValue newValue)
         {
            return onChanging(oldValue, newValue);
         }

         return Wrapped;
      }

      private readonly ConcurrentDictionary<TVisual, HashSet<Action<TVisual, TValue, TValue>>> _changeds;
      private readonly ConcurrentDictionary<TVisual, List<Func<TVisual, TValue, TValue, Boolean>>> _changings;

      private readonly ConcurrentDictionary<TVisual, Func<IVisualElement, Object?>> _computedValues;
      private readonly Object _delegateLock;

      private readonly Int32 _hashCode;
      private readonly ConcurrentDictionary<TVisual, Byte> _knownVisuals;
      private readonly PropertyMetadata _metaData;
      private readonly List<Action<TVisual, TValue, TValue>> _staticChangeds;
      private readonly ConcurrentDictionary<TVisual, ITransition<TValue>> _transitions;
      private readonly ConcurrentDictionary<TVisual, TValue> _values;

      /// <summary>
      ///    Once a property's value has been set by something other than a style (binding, direct set)
      ///    it isn't eligible to be set by a style anymore
      /// </summary>
      private readonly ConcurrentDictionary<TVisual, Boolean> _visualsSetByNotStyle;
   }
}

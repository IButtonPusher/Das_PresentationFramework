using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views
{
    public class DependencyProperty<TVisual, TValue>
        where TVisual : IVisualElement
    {
        public DependencyProperty(String propertyName,
            TValue defaultValue)
        {
            _delegateLock = new Object();
            _propertyName = propertyName;
            _defaultValue = defaultValue;
            _values = new ConcurrentDictionary<TVisual, TValue>();
            _changings = new ConcurrentDictionary<TVisual, List<Func<TVisual, TValue, TValue, Boolean>>>();
            _changeds = new ConcurrentDictionary<TVisual, List<Action<TVisual, TValue, TValue>>>();
            _knownVisuals = new ConcurrentDictionary<TVisual, Byte>();
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

        public TValue GetValue(TVisual forVisual)
        {
            if (!_values.TryGetValue(forVisual, out var good))
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
            if (!_values.TryGetValue(forVisual, out var good))
            {
                good = getDefault(forVisual, contextStyle);
                EnsureKnown(forVisual);
            }

            return good;
        }

        public static DependencyProperty<TVisual, TValue> Register(String propertyName,
            TValue defaultValue)
        {
            return new DependencyProperty<TVisual, TValue>(propertyName, defaultValue);
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

        private IEnumerable<Action<TVisual, TValue, TValue>> GetOnChanged(TVisual forVisual,
                                                                          Action<TVisual, TValue, TValue>? prepend)
        {
            if (prepend != null)
                yield return prepend;

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

            //if (_values.TryGetValue(forVisual, out var was))
            //{
            //    if (Equals(was, value))
            //        return;
            //}
            //else was = _defaultValue;


            //if (_changings.TryGetValue(forVisual, out var interestedParties))
            //{
            //    List<Func<TVisual, TValue, TValue, Boolean>> funcs;
            //    lock (_delegateLock)
            //    {
            //        funcs = new List<Func<TVisual, TValue, TValue, Boolean>>(interestedParties);
            //    }

            //    foreach (var func in funcs)
            //        if (!func(forVisual, was, value))
            //            return;
            //}

            //_values[forVisual] = value;

            //if (!_changeds.TryGetValue(forVisual, out var interested))
            //    return;


            //List<Action<TVisual, TValue, TValue>> actions;
            //lock (_delegateLock)
            //{
            //    actions = new List<Action<TVisual, TValue, TValue>>(interested);
            //}

            //foreach (var func in actions) func(forVisual, was, value);
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
            else was = _defaultValue;


            foreach (var oc in onChangers)
            {
                if (!oc(forVisual, was, value))
                    return;
            }

            _values[forVisual] = value;

            foreach (var oc in onChangeds)
            {
                oc(forVisual, was, value);
            }

            forVisual.RaisePropertyChanged(_propertyName);

            //if (!_changeds.TryGetValue(forVisual, out var interested))
            //    return;


            //List<Action<TVisual, TValue, TValue>> actions;
            //lock (_delegateLock)
            //{
            //    actions = new List<Action<TVisual, TValue, TValue>>(interested);
            //}

            //foreach (var func in actions) func(forVisual, was, value);
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

        private void OnVisualDisposed(IVisualElement visual)
        {
            if (!(visual is TVisual valid))
                return;

            _values.TryRemove(valid, out _);
            _changeds.TryRemove(valid, out _);
            _changings.TryRemove(valid, out _);
        }

        public override String ToString()
        {
            return _propertyName;
        }

        private readonly ConcurrentDictionary<TVisual, List<Action<TVisual, TValue, TValue>>> _changeds;
        private readonly ConcurrentDictionary<TVisual, List<Func<TVisual, TValue, TValue, Boolean>>> _changings;
        private readonly String _propertyName;
        private readonly TValue _defaultValue;
        private readonly Object _delegateLock;
        private readonly ConcurrentDictionary<TVisual, Byte> _knownVisuals;
        private readonly ConcurrentDictionary<TVisual, TValue> _values;
    }
}
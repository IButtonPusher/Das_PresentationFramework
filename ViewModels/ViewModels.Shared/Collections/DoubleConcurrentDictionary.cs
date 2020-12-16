﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AsyncResults.ForEach;

namespace Das.ViewModels.Collections
{
    public class DoubleConcurrentDictionary<TKey1, TKey2, TValue>
    {
        public DoubleConcurrentDictionary()
        {
            _backingDictionary = new ConcurrentDictionary<TKey1, ConcurrentDictionary<TKey2, TValue>>();
        }

        public TValue this[TKey1 k1, TKey2 k2]
        {
            get => _backingDictionary[k1][k2];
            set
            {
                if (!_backingDictionary.TryGetValue(k1, out var d2))
                {
                    d2 = new ConcurrentDictionary<TKey2, TValue>();
                    _backingDictionary[k1] = d2;
                }

                d2[k2] = value;
            }
        }

        public ICollection<Tuple<TKey1, TKey2>> Keys
        {
            get
            {
                var letsUse = new List<Tuple<TKey1, TKey2>>();

                foreach (var kvp in _backingDictionary)
                foreach (var k2 in kvp.Value)
                    letsUse.Add(new Tuple<TKey1, TKey2>(kvp.Key, k2.Key));

                return letsUse;
            }
        }

        public void AddRange<TKeyValue>(TKey1 k1, IEnumerable<TKeyValue> keyVals)
            where TKeyValue : TKey2, TValue
        {
            var updating = _backingDictionary.GetOrAdd(k1, new ConcurrentDictionary<TKey2, TValue>());
            foreach (var kvp in keyVals)
                updating[kvp] = kvp;
        }

        public void Clear()
        {
            _backingDictionary.Clear();
        }

        public Boolean ContainsKey(TKey1 k1)
        {
            return _backingDictionary.TryGetValue(k1, out _);
        }

        public async Task<TValue> GetOrAddAsync(
            TKey1 k1, TKey2 k2,
            Func<TKey1, TKey2, Task<TValue>> bldr)
        {
            var d2 = _backingDictionary.GetOrAdd(k1, _ => new ConcurrentDictionary<TKey2, TValue>());
            //if (d2.TryGetValue(k2, out var gotIt))
            //    return gotIt;

            if (!d2.TryGetValue(k2, out var gotIt))
            {
                gotIt = await bldr(k1, k2);
                d2[k2] = gotIt;
            }

            return gotIt;
            //d2.GetOrAdd(k2, v => await bldr(k1, v));
        }

        public IEnumerable<TValue> GetOrAddValues<TKeyValue>(TKey1 k1, 
                                                             Func<TKey2, Boolean> predicate,
                                                             Func<TKey1, IEnumerable<TKeyValue>> keyVals)
            where TKeyValue : TKey2, TValue
        {
            if (!_backingDictionary.TryGetValue(k1, out var d2))
            {
                var allMyVals = keyVals(k1);
                AddRange(k1, allMyVals);

                if (!_backingDictionary.TryGetValue(k1, out d2))
                    yield break;
            }

            foreach (var kvp in d2)
                if (predicate(kvp.Key))
                    yield return kvp.Value;
        }


        public TValue GetOrAdd(TKey1 k1, 
                               TKey2 k2, 
                               Func<TKey1, TKey2, TValue> factory)
        {
            var innerD = _backingDictionary.GetOrAdd(k1, key1 => new ConcurrentDictionary<TKey2, TValue>());
            return innerD.GetOrAdd(k2, key2 => factory(k1, key2));
        }


        public async IAsyncEnumerable<TValue> GetOrAddValuesAsync<TKeyValue>(
            TKey1 k1, Func<TKey2, Boolean> predicate,
            Func<TKey1, IAsyncEnumerable<TKeyValue>> keyVals)
            where TKeyValue : TKey2, TValue
        {
            if (!_backingDictionary.TryGetValue(k1, out var d2))
            {
                var allMyVals = keyVals(k1);
                var vArr = await allMyVals.ToArrayAsync();
                AddRange(k1, vArr);

                if (!_backingDictionary.TryGetValue(k1, out d2))
                    yield break;
            }

            foreach (var kvp in d2)
                if (predicate(kvp.Key))
                    yield return kvp.Value;
        }

        public IEnumerable<TValue> GetValues(TKey1 k1, Func<TKey2, Boolean> predicate)
        {
            if (!_backingDictionary.TryGetValue(k1, out var d2))
                yield break;

            foreach (var kvp in d2)
                if (predicate(kvp.Key))
                    yield return kvp.Value;
        }

        public void Remove(TKey1 k1)
        {
            _backingDictionary.TryRemove(k1, out _);
        }


        public Boolean TryGetValue(TKey1 k1, TKey2 k2, out TValue value)
        {
            if (_backingDictionary.TryGetValue(k1, out var d2) &&
                d2.TryGetValue(k2, out value))
                return true;

            value = default!;
            return false;
        }

        private readonly ConcurrentDictionary<TKey1, ConcurrentDictionary<TKey2, TValue>> _backingDictionary;
    }
}

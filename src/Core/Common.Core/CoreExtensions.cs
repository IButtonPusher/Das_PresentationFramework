﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Concurrent;

#if !NET40

using TaskEx = System.Threading.Tasks.Task;

#endif


namespace Common.Core
{
    public static class CoreExtensions
    {
        public static Double AddInterlocked(ref Double location1,
                                            Double value)
        {
            var newCurrentValue = location1; // non-volatile read, so may be stale
            while (true)
            {
                var currentValue = newCurrentValue;
                var newValue = currentValue + value;
                newCurrentValue = Interlocked.CompareExchange(ref location1, newValue, currentValue);
                if (newCurrentValue.Equals(currentValue))
                    return newValue;
            }
        }

        public static async Task InvokeAsyncEvent(this Func<Task>? @delegate,
                                                  [CallerMemberName] String? coller = null)
        {
            if (@delegate == null)
                return;

            var invocators = @delegate.GetInvocationList().OfType<Func<Task>>();

            var waitForMeeee = new List<Task>();

            foreach (var inv in invocators)
            {
                waitForMeeee.Add(inv());
            }

         

            await TaskEx.WhenAll(waitForMeeee).ConfigureAwait(false);
        }


        public static TValue GetOrAddNew<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                                 TKey key)
            where TValue : new()
        {
            if (!dictionary.TryGetValue(key, out var val))
            {
                val = new();
                dictionary.Add(key, val);
            }

            return val;
        }

        public static TValue GetOrAddNew<TKey, TValue, TData>(this IDictionary<TKey, TValue> dictionary,
                                                       TKey key,
                                                       TData data,
                                                       Func<TData, TValue> newBldr)
        {
            if (!dictionary.TryGetValue(key, out var val))
            {
                val = newBldr(data);
                dictionary.Add(key, val);
            }

            return val;
        }


        public static Boolean AddValues<TKey, TValKey, TValue, TValues1>(
           this IDictionary<TKey, TValues1> dictionary,
           TKey key,
           TValKey value1,
           TValue value2)
           where TValues1 : IDictionary<TValKey, TValue>, new()
           
        {
           if (!dictionary.TryGetValue(key, out var val))
           {
              val = new TValues1();
              dictionary.Add(key, val);
              val.Add(value1, value2);
              return true;
           }

           val.Add(value1, value2);
           return false;
        }

        public static Boolean AddToValueList<TKey, TValKey, TValue, TValues1, TValues2>(
           this IDictionary<TKey, TValues1> dictionary,
           TKey key,
           TValKey value1,
           TValue value2)
           where TValues1 : IDictionary<TValKey, TValues2>, new()
        where TValues2 : ICollection<TValue>, new()
        {
           if (!dictionary.TryGetValue(key, out var val))
           {
              val = new TValues1();
              dictionary.Add(key, val);


              return AddToValueList(val, value1, value2);
           }

           AddToValueList(val, value1, value2);
           return false;
        }
       
        /// <summary>
        /// Adds a value to a collection of items which is the value of a dictionary
        /// </summary>
        /// <returns>true if <see cref="key"/> didn't already exist in the dictionary</returns>
        public static Boolean AddToValueList<TKey, TValues, TValue>(
            this IDictionary<TKey, TValues> dictionary,
            TKey key,
            TValue value)
            where TValues : ICollection<TValue>, new()
        {
            if (!dictionary.TryGetValue(key, out var val))
            {
                val = new();
                dictionary.Add(key, val);

                val.Add(value);
                return true;
            }

            val.Add(value);
            return false;
        }

        /// <summary>
        /// Adds a value to a collection of items which is the value of a dictionary
        /// </summary>
        /// <returns>true if <see cref="key"/> didn't already exist in the dictionary</returns>
        public static void AddToValueList<TKey, TValues, TValue>(
           this ConcurrentDictionary<TKey, TValues> dictionary,
           TKey key,
           TValue value)
           where TValues : ICollection<TValue>, ICollection, new()
        {
           var vals = dictionary.GetOrAdd(key, _ => new());

           lock (vals.SyncRoot)
              vals.Add(value);

          
        }

        public static void AddToValueList<TKey, TValues, TValue, TNewArg>(
            this IDictionary<TKey, TValues> dictionary,
            TKey key,
            TValue value,
            TNewArg newArg,
            Func<TNewArg, TValues> buildNew)
            where TValues : IList<TValue>
        {
            if (!dictionary.TryGetValue(key, out var val))
            {
                val = buildNew(newArg);
                dictionary.Add(key, val);
            }

            val.Add(value);
        }

        public static async Task InvokeAsyncEvent<TArgs>(this Func<TArgs, Task>? @delegate,
                                                         TArgs args)
        {
            if (@delegate == null)
                return;

            var invocators = @delegate.GetInvocationList().OfType<Func<TArgs, Task>>();

            var waitForMeeee = new List<Task>();

            foreach (var inv in invocators)
            {
                waitForMeeee.Add(inv(args));
            }

            await TaskEx.WhenAll(waitForMeeee).ConfigureAwait(false);
        }

        public static Boolean PathEquals(this FileSystemInfo? info,
                                         FileSystemInfo? other)
        {
            if (ReferenceEquals(null, info))
                return ReferenceEquals(null, other);

            if (ReferenceEquals(null, other))
                return false;

            if (info.GetType() != other.GetType())
                return false;

            var l = info.FullName.TrimEnd('\\');
            var r = other.FullName.TrimEnd('\\');
            var res = String.Equals(l, r, StringComparison.OrdinalIgnoreCase);

            return res;

            //return String.Equals(info.FullName.TrimEnd('\\'), other.FullName.TrimEnd('\\'),
            //    StringComparison.OrdinalIgnoreCase);
        }
        

        public static TDic GetIntersection<TDic>(this TDic left,
                                                               TDic right)
            where TDic : IDictionary, new()
        {
            var res = new TDic();

            foreach (var key in left.Keys)
            {
                if (!right.Contains(key))
                    continue;

                if (!Equals(left[key], right[key]))
                    continue;

                res.Add(key, left[key]);
            }

            return res;
        }


        //public static Boolean AreDictionariesEqual<TKey, TValue>(IDictionary<TKey, TValue> left,
        //                                                         IDictionary<TKey, TValue> right)

        //    where TValue : IEquatable<TValue>
        //{
        //    if (left.Count != right.Count)
        //        return false;

        //    foreach (var kvp in left)
        //    {
        //        if (!right.TryGetValue(kvp.Key, out var rValue) || 
        //            !rValue.Equals(kvp.Value))
        //            return false;
        //    }

        //    return true;
        //}

      #if !NET40

        public static Boolean AreDictionariesEqual<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> left,
                                                                 IReadOnlyDictionary<TKey, TValue> right)

            where TValue : IEquatable<TValue>
        {
            if (left.Count != right.Count)
                return false;

            foreach (var kvp in left)
            {
                if (!right.TryGetValue(kvp.Key, out var rValue) || 
                    !rValue.Equals(kvp.Value))
                    return false;
            }

            return true;
        }

         #endif

        public static async Task HandleCollectionChangedAsync<T>(this NotifyCollectionChangedEventArgs e,
                                                                 Func<T, Task> handleOldItem,
                                                                 Func<T, Task> handleNewItem)
        {
            if (e.OldItems is { } oi)
            {
                foreach (var item in oi.OfType<T>())
                {
                    await handleOldItem(item);
                }
            }

            if (e.NewItems is { } ni)
            {
                foreach (var item in ni.OfType<T>())
                {
                    await handleNewItem(item);
                }
            }
        }

        public static void HandleCollectionChanged<T>(this NotifyCollectionChangedEventArgs e,
                                                      Action<T> handleOldItem,
                                                      Action<T> handleNewItem)
        {
           if (e.OldItems is { } oi)
           {
              foreach (var item in oi.OfType<T>())
              {
                 handleOldItem(item);
              }
           }

           if (e.NewItems is { } ni)
           {
              foreach (var item in ni.OfType<T>())
              {
                 handleNewItem(item);
              }
           }
        }

        // ReSharper disable once UnusedMember.Global
        public static IEnumerable<T[]> Combinations<T>(this IList<T> argList,
                                                       Int32 argSetSize)
        {
            return CombinationsImpl(argList, 0, argSetSize - 1);
        }

        public static void AddRange<T>(this ICollection<T> collection,
                                       IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Add(item);
        }


        public static Boolean ContainsAny(this String str,
                                          params String[] list)
        {
            foreach (var s in list)
                if (str.Contains(s))
                    return true;

            return false;
        }


        public static Boolean ContainsAny<T>(this T[] items,
                                             params T[] list)
        {
            foreach (var item in items)
            {
                if (list.Contains(item))
                    return true;
            }

            return false;
        }

        public static void Dispose(this IEnumerable<IDisposable> manyDisposables)
        {
            var safe = manyDisposables.ToArray();
            for (var c = 0; c < safe.Length; c++)
            {
                safe[c]?.Dispose();
            }
        }

       

        public static IEnumerable<TEnum> GetLegitValues<TEnum>()
            where TEnum : struct, IConvertible
        {
            var t = typeof(TEnum);

            if (!t.IsEnum)
            {
                if (t == typeof(Boolean))
                {
                    var b = true;
                    if (b is TEnum be)
                        yield return be;

                    b = false;

                    if (b is TEnum be2)
                        yield return be2;

                    yield break;
                }

                throw new InvalidCastException();
            }

            var vals = Enum.GetValues(t);

            foreach (TEnum val in vals)
            {
                if (!val.HasAttribute<TEnum, UnsetEnumValueAttribute>())
                    yield return val;
            }
        }

        public static Boolean HasAttribute<TEnum, TAttribute>(this TEnum value)
            where TEnum : struct, IConvertible
            where TAttribute : Attribute
        {
            foreach (var _ in GetAttributesForEnumValue<TEnum, TAttribute>(value))
                return true;

            return false;
        }

        public static Boolean HasAttribute<TAttribute>(this IConvertible value)
            where TAttribute : Attribute
        {
            foreach (var _ in GetAttributesForEnumValue<TAttribute>(value, value.GetType()))
                return true;

            return false;
        }

        public static Boolean TryGetAttribute<TAttribute>(this MemberInfo member,
                                                          out TAttribute attribute)
            where TAttribute : Attribute
        {
            var attributes = member.GetCustomAttributes(typeof(TAttribute), true);
            foreach (var attr in attributes.OfType<TAttribute>())
            {
                attribute = attr;
                return true;
            }

            attribute = default!;
            return false;
        }

        public static IEnumerable<TAttribute> GetAttributesForEnumValue<TEnum, TAttribute>(TEnum value)
            where TEnum : struct, IConvertible
            where TAttribute : Attribute
        {
            return GetAttributesForEnumValue<TAttribute>(value, typeof(TEnum));
        }

        public static IEnumerable<TAttribute> GetAttributesForEnumValue<TAttribute>(IConvertible? value,
            Type enumType)
            where TAttribute : Attribute
        {
            if (value == null)
                yield break;

            var vStr = value.ToString(CultureInfo.InvariantCulture);

            var fi = enumType.GetField(vStr);

            if (fi == null)
                yield break;

            var attributes = fi.GetCustomAttributes(typeof(TAttribute), true);

            foreach (var attr in attributes.OfType<TAttribute>())
                yield return attr;
        }

        #if NET40

        public static String GetRunningFrameworkVersion()
        {
            var strType = typeof(String);
#pragma warning disable CS0618
            var assemblyUri = strType.Assembly.CodeBase!;

            var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(
                new Uri(assemblyUri).LocalPath);
            return versionInfo.ProductVersion!;
#pragma warning restore CS0618
        }

        #endif


        public static String ToCommaString<T>(this IEnumerable<T> list)
        {
            return list.ToDelimitedString(',');
        }

        public static String ToDelimitedString<T>(this IEnumerable<T>? list,
                                                  Char delimiter)
        {
            if (list == null)
                return String.Empty;

            var output = new StringBuilder();
            var didFind = false;

            foreach (var item in list)
            {
                didFind = true;
                output.Append(item);
                output.Append(delimiter);
            }

            if (!didFind)
                return output.ToString();

            output.Remove(output.Length - 1, 1);
            return output.ToString();
        }

        //[MethodImpl(256)]
        //public static Boolean AreEqualEnough(this Double f1,
        //                                     Double f2)
        //{
        //   return Math.Abs(f1 - f2) < 0.00001;
        //}

        public static String ToMultiline(this String input,
                                         Int32 rowLength)
        {
            var result = new StringBuilder();

            var currentRowLength = 0;
            foreach (var c in input)
            {
                switch (c)
                {
                    case ' ':
                    case '\t':
                        if (currentRowLength >= rowLength)
                        {
                            result.Append("\r\n");
                            currentRowLength = 0;
                            continue;
                        }

                        break;
                    case '\r':
                    case '\n':

                        currentRowLength = 0;
                        break;
                    default:
                        currentRowLength++;
                        break;
                }

                result.Append(c);
            }

            return result.ToString();
        }

        private static IEnumerable<T[]> CombinationsImpl<T>(IList<T> argList,
                                                            Int32 argStart,
                                                            Int32 argIteration,
                                                            List<Int32>? argIndicies = null)
        {
            argIndicies ??= new List<Int32>();
            for (var i = argStart; i < argList.Count; i++)
            {
                argIndicies.Add(i);
                if (argIteration > 0)
                {
                    foreach (var array in CombinationsImpl(argList, i + 1, argIteration - 1, argIndicies))
                        yield return array;
                }
                else
                {
                    var array = new T[argIndicies.Count];
                    for (var j = 0; j < argIndicies.Count; j++) array[j] = argList[argIndicies[j]];

                    yield return array;
                }

                argIndicies.RemoveAt(argIndicies.Count - 1);
            }
        }
    }
}

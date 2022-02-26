using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Styles;

namespace Das.Views
{
    public static class ExtensionMethods
    {
        public static void Clear<T>(this IProducerConsumerCollection<T> q)
        {
            while (q.TryTake(out _))
            {
            }
        }

        public static void Clear<T>(this BlockingCollection<T> bloc)
        {
            while (bloc.Count > 0) bloc.TryTake(out _);
        }

        public static Boolean Contains(this VisualStateType type,
                                       VisualStateType value)
        {
            return (type & value) > VisualStateType.Active;
        }

        public static TEnum GetEnumValue<TEnum>(String name,
                                                TEnum defaultValue)
            where TEnum : struct
        {
            return GetEnumValue(name, defaultValue, true);
        }

        public static TEnum GetEnumValue<TEnum>(String name,
                                                TEnum defaultValue,
                                                Boolean isThrowifInvalid)
            where TEnum : struct
        {
            if (name.Length == 0)
                return defaultValue;

            name = name.IndexOf('-') > 0
                ? name.Replace("-", "")
                : name;

            if (Enum.TryParse<TEnum>(name, true, out var value))
                return value;

            if (!isThrowifInvalid)
                return defaultValue;

            throw new InvalidOperationException();
        }

        public static void HandleCollectionChange<T>(this NotifyCollectionChangedEventArgs e,
                                                     Action<T> oldItems,
                                                     Action<T> newItems)
        {
            if (e.OldItems != null)
            {
                var olds = e.OldItems.OfType<T>();
                foreach (var old in olds)
                {
                    oldItems(old);
                }
            }

            if (e.NewItems == null)
                return;

            var news = e.NewItems.OfType<T>();
            foreach (var n in news)
            {
                newItems(n);
            }
        }

        

        public static String ToSvgString(this Double value)
        {
            // Use G7 format specifier to be compatible across all target frameworks.
            return value.ToString("G7", CultureInfo.InvariantCulture);
        }

        public static String ToSvgString(this Single value)
        {
            // Use G7 format specifier to be compatible across all target frameworks.
            return value.ToString("G7", CultureInfo.InvariantCulture);
        }

        public static String ToSvgString(this IPoint2D p)
        {
            return p.X.ToSvgString() + " " + p.Y.ToSvgString();
        }

        public static String ToSvgString(this IPoint2F p)
        {
            return p.X.ToSvgString() + " " + p.Y.ToSvgString();
        }

        public static Boolean TryGetEnumValue<TEnum>(String name,
                                                     out TEnum value)
            where TEnum : struct
        {
            if (name.Length == 0)
                goto fail;

            name = name.IndexOf('-') > 0
                ? name.Replace("-", "")
                : name;

            if (Enum.TryParse<TEnum>(name, true, out var val))
            {
                value = val;
                return true;
            }

            fail:
            value = default!;
            return false;
        }
    }
}

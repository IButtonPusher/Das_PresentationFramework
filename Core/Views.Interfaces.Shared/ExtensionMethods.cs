﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
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

        public static void HandleCollectionChange<T>(this NotifyCollectionChangedEventArgs e,
                                                     Action<T> oldItems,
                                                     Action<T> newItems)
        {
            if (e.OldItems != null)
            {
                var olds = e.OldItems.OfType<T>();
                foreach (var old in olds)
                    oldItems(old);
            }

            if (e.NewItems == null)
                return;

            var news = e.NewItems.OfType<T>();
            foreach (var n in news)
                newItems(n);
        }

        public static void HandleCollectionChanges<T>(this NotifyCollectionChangedEventArgs e,
                                                      Action<IEnumerable<T>> oldItems,
                                                      Action<IEnumerable<T>> newItems,
                                                      Action? onClear = null)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (onClear != null)
                    onClear();

                return;
            }

            if (e.OldItems != null)
            {
                var olds = e.OldItems.OfType<T>();
                oldItems(olds);
            }

            if (e.NewItems == null)
                return;

            var news = e.NewItems.OfType<T>();
            newItems(news);
        }

        public static Boolean Contains(this StyleSelector selector,
                                       StyleSelector value)
        {
            return (selector & value) > StyleSelector.Active;
        }
    }
}
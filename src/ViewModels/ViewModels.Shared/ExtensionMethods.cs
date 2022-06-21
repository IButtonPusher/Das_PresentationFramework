using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Das.Mvvm
{
    public static class ExtensionMethods
    {
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
    }
}

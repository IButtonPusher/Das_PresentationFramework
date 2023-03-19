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
               onClear?.Invoke();

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

        public static async Task HandleCollectionChangesAsync<T>(this NotifyCollectionChangedEventArgs e,
                                                      Func<IEnumerable<T>, Task> oldItems,
                                                      Func<IEnumerable<T>, Task> newItems,
                                                      Func<Task>? onClear = null)
        {
           if (e.Action == NotifyCollectionChangedAction.Reset)
           {
              if (onClear != null)
                 await onClear();

              return;
           }

           if (e.OldItems != null)
           {
              var olds = e.OldItems.OfType<T>();
              await oldItems(olds);
           }

           if (e.NewItems == null)
              return;

           var news = e.NewItems.OfType<T>();
           await newItems(news);
        }
    }
}

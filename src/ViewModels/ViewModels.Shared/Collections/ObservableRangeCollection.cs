using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Mvvm;

namespace Das.ViewModels
{
    public class ObservableRangeCollection<T> : ObservableCollection<T>,
                                                INotifyingCollection<T>
    {
       public ObservableRangeCollection(IEnumerable<T> collection) : base(collection)
       {
          _itemLock = new Object();
       }

        public ObservableRangeCollection()
        {
            _itemLock = new Object();
        }

        private  Int32 SuspendDown()
        {
           return Interlocked.Decrement(ref __suspendCounter);
        }

        private void SuspendUp()
        {
           Interlocked.Increment(ref __suspendCounter);
        }

        Object? INotifyingCollection.this[Int32 index] => this[index];

        public void AddRange(IEnumerable<T> items, 
                             Boolean isClearCurrentItems = false)
        {
           SuspendUp();

           T[] arr;

           try
           {

              lock (_itemLock)
              {
                 if (isClearCurrentItems)
                 {
                    foreach (var item in this.OfType<IDisposable>())
                    {
                       item.Dispose();
                    }

                    Clear();
                 }

                 arr = items.ToArray();
                 foreach (var item in arr)
                    Add(item);
              }
           }
           finally
           {
              SuspendDown();
           }

           OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                arr));
        }


        protected sealed override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (__suspendCounter > 0)
                return;

            base.OnCollectionChanged(e);
        }

        
        private readonly Object _itemLock;
        private Int32 __suspendCounter;
    }
}

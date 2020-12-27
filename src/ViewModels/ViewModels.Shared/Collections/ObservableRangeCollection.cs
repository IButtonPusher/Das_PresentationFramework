using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Das.Views.Mvvm;

namespace Das.ViewModels
{
    public class ObservableRangeCollection<T> : ObservableCollection<T>,
                                                INotifyingCollection<T>
    {
        public ObservableRangeCollection()
        {
            _itemLock = new Object();
        }

        Object? INotifyingCollection.this[Int32 index] => this[index];

        public void AddRange(IEnumerable<T> items, 
                             Boolean isClearCurrentItems = false)
        {
            T[] arr;

            lock (_itemLock)
            {
                _isSuspendEvents = true;
                if (isClearCurrentItems)
                    Clear();

                arr = items.ToArray();
                foreach (var item in arr)
                    Add(item);

                _isSuspendEvents = false;
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                arr));
        }


        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_isSuspendEvents)
                return;

            base.OnCollectionChanged(e);
        }

        private Boolean _isSuspendEvents;
        private readonly Object _itemLock;
    }
}

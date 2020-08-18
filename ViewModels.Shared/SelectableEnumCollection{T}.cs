using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Das.Mvvm
{
    public class SelectableEnumCollection<T> : ObservableCollection<Selectable<T>>, IDisposable
        where T : Enum
    {
        public SelectableEnumCollection(Int32 minEnumValue)
            : this(minEnumValue, Enumerable.Empty<T>())
        {
        }

        public SelectableEnumCollection(Int32 minEnumValue, IEnumerable<T> initiallySelected)
        {
            _isSuspendEvents = true;

            var available = Enum.GetValues(typeof(T)).OfType<T>();

            var searchSelected = new HashSet<T>(initiallySelected);
            foreach (var item in available)
            {
                if (Convert.ToInt32(item) < minEnumValue)
                    continue;

                var vm = new Selectable<T>(item)
                {
                    IsSelected = searchSelected.Contains(item)
                };
                vm.PropertyChanged += OnPropertyChanged;
                Add(vm);
            }

            _isSuspendEvents = false;
        }


        public void Dispose()
        {
            foreach (var item in this)
            {
                item.PropertyChanged -= OnPropertyChanged;

                if (item is IDisposable disposable)
                    disposable.Dispose();
            }
        }

        public IEnumerable<T> SelectedItems
        {
            get => from s in this
                where s.IsSelected
                select s.Item;

            set => SetSelected(value);
        }

        public void SetSelectionAll(Boolean isSelected)
        {
            SetSelected(isSelected 
                ? this.Select(i => i.Item) 
                : Enumerable.Empty<T>());
        }

        private void OnPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            if (_isSuspendEvents)
                return;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
        }

        private void SetSelected(IEnumerable<T> items)
        {
            var searchSelected = new HashSet<T>(items);
            foreach (var item in this)
                item.IsSelected = searchSelected.Contains(item.Item);
        }

        private readonly Boolean _isSuspendEvents;
    }
}
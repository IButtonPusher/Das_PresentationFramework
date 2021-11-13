using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Views;

// ReSharper disable VirtualMemberCallInConstructor

namespace Das.Mvvm
{
   public class SelectableCollection<T> : ObservableCollection<Selectable<T>>,
                                          IDisposable
   {
      public SelectableCollection(IEnumerable<Selectable<T>> items)
         : base(items)
      {
         foreach (var item in this)
         {
            item.PropertyChanged += OnSelectableValueChanged;
         }

         CollectionChanged += OnThisHereCollectionChanged;
      }

      public SelectableCollection(IEnumerable<T> available,
                                  IEnumerable<T> initiallySelected,
                                  Boolean isSelectAllIfNoneSelected)
      {
         var searchSelected = new HashSet<T>(initiallySelected);

         isSelectAllIfNoneSelected &= searchSelected.Count == 0;

         foreach (var item in available)
         {
            var vm = new Selectable<T>(item,
               isSelectAllIfNoneSelected || searchSelected.Contains(item));

            vm.PropertyChanged += OnSelectableValueChanged;

            Add(vm);
         }

         CollectionChanged += OnThisHereCollectionChanged;
      }

      public IEnumerable<T> GetSelectedItems()
      {
         foreach (var item in this)
         {
            if (!item.IsSelected)
               continue;

            yield return item.Item;
         }
      }

      public Boolean IsAllSelected
      {
         get
         {
            foreach (var item in this)
            {
               if (!item.IsSelected)
                  return false;
            }

            return true;
         }
      }

      private void OnThisHereCollectionChanged(Object sender,
                                               NotifyCollectionChangedEventArgs e)
      {
         e.HandleCollectionChanges<Selectable<T>>(oldies =>
         {
            foreach (var o in oldies)
               o.PropertyChanged -= OnSelectableValueChanged;

         }, newbies =>
         {
            foreach (var o in newbies)
               o.PropertyChanged += OnSelectableValueChanged;
         });
      }

      public event Action<Selectable<T>>? SelectionChanged;

      private void OnSelectableValueChanged(Object sender,
                                            PropertyChangedEventArgs e)
      {
         if (e.PropertyName == nameof(Selectable<T>.IsSelected) &&
             sender is Selectable<T> good)
            SelectionChanged?.Invoke(good);
      }


      public void Dispose()
      {
         SelectionChanged = null;
         foreach (var item in this)
         {
            item.PropertyChanged -= OnSelectableValueChanged;
         }
      }
   }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Das.Mvvm
{
    public class SelectableCollection<T> : ObservableCollection<Selectable<T>>
        where T : IEquatable<T>
    {
        public SelectableCollection(IEnumerable<T> available, IEnumerable<T> initiallySelected)
        {
            var searchSelected = new HashSet<T>(initiallySelected);
            foreach (var item in available)
            {
                var vm = new Selectable<T>(item)
                {
                    IsSelected = searchSelected.Contains(item)
                };
                Add(vm);
            }
        }
    }

    
}

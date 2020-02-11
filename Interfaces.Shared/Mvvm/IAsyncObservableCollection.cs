using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Das.ViewsModels
{
    public interface IAsyncObservableCollection<T> : 
        INotifyCollectionChanged, INotifyPropertyChanged, ICollection<T>
        where T : IEquatable<T>
    {
        Task<Boolean> AddAsync(T item);
        Task AddOrUpdate(T item);
        Task<Boolean> TryRemove(T item);
        Task ClearAsync();
        Task<IReadOnlyList<T>> SelectAsync(Func<T, Boolean> predicate);
        Task<T> FirstOrDefault(Func<T, Boolean> predicate);
        Task<Boolean> Any(Func<T, Boolean> predicate);
        Task<Boolean> AllAsync(Func<T, Boolean> predicate);
    }
}

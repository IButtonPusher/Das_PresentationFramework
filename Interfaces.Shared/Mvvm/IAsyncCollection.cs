using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.ViewsModels
{
    public interface IAsyncCollection<T> : IAsyncEnumerable<T>
    {
        Task<T[]> ToArrayAsync();

        IAsyncEnumerable<T> EnumerateAsync();

        Task<Boolean> AddAsync(T item);

        Task AddAsync(IEnumerable<T> items);

        Task InsertAsync(Int32 index, T item);

        Task RemoveAtAsync(Int32 index);
    }
}

﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.ViewModels
{
    public interface IAsyncCollection<T> : IAsyncEnumerable<T>,
                                           IAsyncQueryable<T>
    {
        Task<Boolean> AddAsync(T item);

        Task AddAsync(IEnumerable<T> items);

        IAsyncEnumerable<T> EnumerateAsync();

        Task<T> FirstOrDefaultAsync<TDerived>(Func<TDerived, Boolean> predicate)
            where TDerived : class, T;

        Task InsertAsync(Int32 index,
                         T item);

        Task RemoveAtAsync(Int32 index);

        Task<T[]> ToArrayAsync();
    }
}

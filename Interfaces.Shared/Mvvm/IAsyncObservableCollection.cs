using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Das.ViewsModels
{
    public interface IAsyncObservableCollection<T> :
        INotifyCollectionChanged, INotifyPropertyChanged, ICollection<T>,
        IAsyncCollection<T>, IDisposable
        where T : IEquatable<T>
    {
        T this[Int32 index] { get; }

        Task AddRangeAsync(IEnumerable<T> items);

        Task<Boolean> AddOrUpdateAsync(T item);

        Task<Boolean> AllAsync(Func<T, Boolean> predicate);

        Task<Boolean> AnyAsync(Func<T, Boolean> predicate);

        Task ClearAsync();

        Task<Boolean> ContainsAsync(T item);


        /// <summary>
        ///     Suspends events during the delegate invocation then
        ///     fires a reset event upon completion
        /// </summary>
        Task DoAsyncTransaction(Func<IAsyncObservableCollection<T>, Task> action);

        Task DoTransaction(Action<IAsyncObservableCollection<T>> action);

        Task DoTransaction<TInput>(IEnumerable<TInput> datas, 
                                   Action<IAsyncObservableCollection<T>, TInput> action);

        Task<T> FirstOrDefaultAsync(Func<T, Boolean> predicate);

        Task<T> FirstOrDefaultAsync();

        Task<IReadOnlyList<T>> GetEnumerableAsync();

        Task<Int32> IndexOfAsync(T item);

        Task Remove(IEnumerable<T> items);

        Task RunOnEach(Action<T> action);

        IAsyncEnumerable<TRes> GetFromEach<TRes>(Func<T, TRes> action);

        Task<IReadOnlyList<T>> SelectAsync(Func<T, Boolean> predicate);

        Task<IReadOnlyList<T>> SelectAsync<TArg>(Func<T, TArg, Boolean> predicate, TArg data);

        Task<TCollection> SelectAsync<TCollection, TOut>(
            Func<T, Boolean> predicate, Func<T, TOut> selecting)
            where TCollection : ICollection<TOut>, new();

        Task Synchonize<TOther>(NotifyCollectionChangedEventArgs args,
            Func<T, TOther, Boolean> equate,
            Func<TOther, T> forNew);

        Task Synchonize<TOther>(IEnumerable<TOther> args,
            Func<T, TOther, Boolean> equate,
            Func<TOther, T> forNew);

        Task Synchronize(IEnumerable<T> items);

        Task<Boolean> TryRemove(T item);

        Task TryRemove(IEnumerable<T> item);

        Task<Boolean> TryRemove(ICollection<T> item, IProgress<T> progress);
    }
}
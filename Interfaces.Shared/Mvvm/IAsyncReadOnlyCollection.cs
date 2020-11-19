using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Das.ViewModels
{
    public interface IAsyncReadOnlyCollection<out T> : IEnumerable<T>,
                                                       INotifyCollectionChanged,
                                                       ICollection
    {
        T this[Int32 index] { get; }
    }
}
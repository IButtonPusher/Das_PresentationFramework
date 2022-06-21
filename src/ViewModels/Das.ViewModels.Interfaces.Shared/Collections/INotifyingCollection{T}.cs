using System;
using System.Collections.Generic;

namespace Das.Views.Mvvm
{
    public interface INotifyingCollection<out T> : INotifyingCollection,
                                                   IEnumerable<T>
    {
        new T this[Int32 index] { get; }
    }
}

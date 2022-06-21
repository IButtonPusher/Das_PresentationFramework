using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Das.ViewModels
{
   public interface IAsyncReadOnlyCollection<T> : IEnumerable<T>,
                                                  INotifyCollectionChanged,
                                                  IAsyncQueryable<T>,
                                                  ICollection
   {
      T this[Int32 index] { get; }
   }
}

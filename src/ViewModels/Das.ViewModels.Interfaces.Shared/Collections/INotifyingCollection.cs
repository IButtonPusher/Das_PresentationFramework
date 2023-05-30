using System;
using System.Collections;
using System.Collections.Specialized;

namespace Das.Views.Mvvm;

public interface INotifyingCollection : INotifyCollectionChanged,
                                        IEnumerable
{
   Object? this[Int32 index] { get; }
}
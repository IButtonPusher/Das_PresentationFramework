using System;
using System.Threading.Tasks;

namespace System.Collections.Generic;

public class ListEx<T> : List<T>
    #if NET40
                            , IReadOnlyList<T>
    #else
    , IReadOnlyIndexer<T>
    #endif
{

    public static IReadOnlyList<T> Empty = new ListEx<T>(0);

    public ListEx(IEnumerable<T> items) : base(items)
    {
    }

    public ListEx(Int32 size) : base(size)
    {
    }

    public ListEx()
    {

    }

    public void TrimToLast(Int32 count)
    {
        var end = Count - count;
        if (end <= 0)
            return;

        RemoveRange(0, end);
    }

    public static implicit operator T[](ListEx<T> list)
    {
        return list.ToArray();
    }

    public Int32 GetCount() => Count;

}
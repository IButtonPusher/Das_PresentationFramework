using System;
using System.Collections.Generic;
using Das.Views.Core.Drawing;

namespace Das.Views.Charting
{
    public interface IPieData<TKey, out TValue> where TValue : IConvertible
    {
        IEnumerable<IDataPoint<TKey, TValue>> Items { get; }

        IDictionary<TKey, IBrush> ItemColors { get; }
    }
}

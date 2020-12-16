using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;

namespace Das.Views.Charting
{
    public interface IPieData<TKey, out TValue> 
        where TValue : IConvertible
    {
        IDictionary<TKey, IBrush> ItemColors { get; }

        IEnumerable<IDataPoint<TKey, TValue>> Items { get; }
    }
}
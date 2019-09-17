using System;

namespace Das.Views.Charting
{
    /// <summary>
    /// Type for a data point that exists on a chart.  The way the point data is formatted as a
    /// string may depend on the TValue
    /// </summary>
    public interface IDataPoint<out TKey, out TValue> where TValue : IConvertible
    {
        TKey Description { get; }

        TValue Value { get; }
    }
}

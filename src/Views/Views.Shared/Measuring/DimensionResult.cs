using System;
using System.Threading.Tasks;

namespace Das.Views.Measuring
{
    [Flags]
    public enum DimensionResult
    {
        Invalid = -1,
        None = 0,
        Width = 1,
        Height = 2,
        HeightAndWidth = 3
    }
}

using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Drawing
{
    public interface IBrush : IEquatable<IBrush>
    {
        Boolean IsInvisible { get; }
        
        Double Opacity { get; }

        IBrush GetWithOpacity(Double opacity);
    }
}
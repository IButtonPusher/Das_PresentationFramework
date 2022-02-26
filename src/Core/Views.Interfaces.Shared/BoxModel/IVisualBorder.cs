using System;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Views.BoxModel
{
    public interface IVisualBorder : IBoxValue<IBrush>
    {
        IThickness GetThickness<TSize>(TSize available)
            where TSize : ISize;

        Boolean IsEmpty { get; }
    }
}

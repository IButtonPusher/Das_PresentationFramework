using System;
using Das.Views.Rendering;

namespace Das.Views
{
    public interface IRenderKit //: IInputHandler, IChangeTracking
    {
        //IInputContext InputContext { get; }

        IMeasureContext MeasureContext { get; }

        IRenderContext RenderContext { get; }

        //IStyleContext StyleContext { get; }
    }
}
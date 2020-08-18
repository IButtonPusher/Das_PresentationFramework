using Das.Views.Input;
using Das.Views.Rendering;

namespace Das.Views
{
    public interface IRenderKit
    {
        IMeasureContext MeasureContext { get; }

        IRenderContext RenderContext { get; }

        //IInputContext InputContext { get; }
    }
}
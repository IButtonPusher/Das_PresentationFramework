using System;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views
{
    public interface IRenderKit
    {
        IMeasureContext MeasureContext { get; }

        IRenderContext RenderContext { get; }
    }
}
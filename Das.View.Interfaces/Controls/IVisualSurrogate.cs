using System;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views.Controls
{
    /// <summary>
    ///     A visual that requires a platform specific implementation
    /// </summary>
    public interface IVisualSurrogate : IVisualElement
    {
        Type ReplacesType { get; }

        //ISize Measure(IVisualElement element,
        //              ISize availableSpace,
        //              IMeasureContext measureContext);

        //void Arrange(IVisualElement element,
        //             ISize availableSpace, 
        //             IRenderContext renderContext);
    }
}
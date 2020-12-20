using System;
using System.Threading.Tasks;

namespace Das.Views.Controls
{
    /// <summary>
    ///     A visual that requires a platform specific implementation
    /// </summary>
    public interface IVisualSurrogate : IVisualElement
    {
        //Type ReplacesType { get; }
        
        IVisualElement ReplacingVisual { get; }
    }
}
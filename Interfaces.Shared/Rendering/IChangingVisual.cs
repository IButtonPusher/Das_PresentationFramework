using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Das.Views.Rendering
{
    // ReSharper disable once UnusedType.Global
    public interface IChangingVisual : IVisualElement, IChangeTracking
    {
    }
}
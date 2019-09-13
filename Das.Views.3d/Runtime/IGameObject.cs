using System.Collections.Generic;

namespace Das.Views.Extended.Runtime
{
    public interface IGameObject<out TViewModel>
        where TViewModel : I3dViewModel
    {
        IEnumerable<IVisual3dElement> VisualElements { get; }

        TViewModel DataContext { get; }
    }
}

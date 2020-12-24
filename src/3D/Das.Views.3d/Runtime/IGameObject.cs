using System.Collections.Generic;

namespace Das.Views.Extended.Runtime
{
    public interface IGameObject<out TViewModel>
        where TViewModel : I3dViewModel
    {
        IEnumerable<IMesh> VisualElements { get; }

        TViewModel DataContext { get; }
    }
}

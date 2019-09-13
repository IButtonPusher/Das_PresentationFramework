using System.Collections.Generic;

namespace Das.Views.Extended.Runtime
{
    public interface IScene
    {
        IEnumerable<IVisual3dElement> VisualElements { get; }
    }
}

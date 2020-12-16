using System.Collections.Generic;

namespace Das.Views.Extended.Runtime
{
    public interface IScene
    {
        IEnumerable<IMesh> VisualElements { get; }
    }
}

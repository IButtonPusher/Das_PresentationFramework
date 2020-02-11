using Das.Views.Extended.Core;
using Das.ViewsModels;

namespace Das.Views.Extended
{
    public interface ISceneViewModel : IViewModel
    {
        ICamera Camera { get; }

        void Update();
    }
}

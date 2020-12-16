using Das.Views.Mvvm;

namespace Das.Views.Extended
{
    public interface ISceneViewModel : IViewModel
    {
        ICamera Camera { get; }

        void Update();
    }
}

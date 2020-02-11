using Das.Views.Panels;
using Das.ViewsModels;

namespace Das.Views
{
    public interface IBootStrapper
    {
        void Run<TViewModel>(TViewModel viewModel, IView view)
            where TViewModel : IViewModel;
    }
}

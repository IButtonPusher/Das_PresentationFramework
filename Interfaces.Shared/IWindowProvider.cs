using Das.Views.Panels;
using Das.ViewsModels;

namespace Das.Views
{
    public interface IWindowProvider<out TWindow> where TWindow : IViewHost
    {
        TWindow Show<TViewModel>(TViewModel viewModel, IView view)
            where TViewModel : IViewModel;
    }
}

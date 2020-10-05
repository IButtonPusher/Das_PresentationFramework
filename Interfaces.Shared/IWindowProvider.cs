using System;
using System.Threading.Tasks;
using Das.Views.Panels;
using Das.ViewModels;

namespace Das.Views
{
    public interface IWindowProvider<out TWindow>
        where TWindow : IVisualHost
    {
        TWindow Show<TViewModel>(TViewModel viewModel, IView view)
            where TViewModel : IViewModel;

        TWindow Show<TViewModel>(TViewModel viewModel, IView<TViewModel> view)
            where TViewModel : IViewModel;

        event Action<TWindow>? WindowShown;
    }
}
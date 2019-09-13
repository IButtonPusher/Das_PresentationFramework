using System;
using Das.Views.Panels;

namespace Das.Views
{
    public interface IWindowProvider<out TWindow> where TWindow : IViewHost
    {
        TWindow Show<TViewModel>(TViewModel viewModel, IView view)
            where TViewModel : IViewModel;
    }
}

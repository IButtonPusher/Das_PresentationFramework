using System;
using System.Threading.Tasks;
using Das.Views.DataBinding;
using Das.Views.Mvvm;
using Das.Views.Panels;

namespace Das.Views
{
    public interface IWindowProvider<out TWindow> : IWindowNotifier<TWindow>
        where TWindow : IVisualHost
    {
        TWindow Show<TViewModel>(TViewModel viewModel,
                                 IBindableElement view);
            //where TViewModel : IViewModel;

            //TWindow Show<TViewModel>(TViewModel viewModel,
            //                         IView<TViewModel> view);
            //where TViewModel : IViewModel;

        TWindow Show<TViewModel>(IView view);

        //where TViewModel : IViewModel;
    }
}
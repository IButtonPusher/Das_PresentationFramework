using System;
using System.Threading.Tasks;
using Das.Views.Panels;
using Das.ViewModels;

namespace Das.Views
{
    public interface IBootStrapper
    {
        void Run<TViewModel>(TViewModel viewModel, IView view)
            where TViewModel : IViewModel;
    }
}
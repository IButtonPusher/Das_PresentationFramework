using System;
using Das.Views.Panels;

namespace Das.Views
{
    public interface IBootStrapper
    {
        void Run<TViewModel>(TViewModel viewModel, IView view)
            where TViewModel : IViewModel;
    }
}

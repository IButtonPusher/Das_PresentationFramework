using System;
using System.Threading.Tasks;
using Das.Views.Mvvm;
using Das.Views.Panels;

namespace Das.Views
{
    public interface IBootStrapper
    {
        void Run<TViewModel>(TViewModel viewModel, IView view)
            where TViewModel : IViewModel;

        IVisualBootStrapper VisualBootStrapper { get; }
    }
}
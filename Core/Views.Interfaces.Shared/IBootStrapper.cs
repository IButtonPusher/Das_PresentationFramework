using System;
using System.Threading.Tasks;
using Das.Views.DataBinding;
using Das.Views.Mvvm;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Views
{
    public interface IBootStrapper
    {
        void Run<TViewModel>(TViewModel viewModel,
                             IView view);
            

        IVisualBootstrapper VisualBootstrapper { get; }
    }
}
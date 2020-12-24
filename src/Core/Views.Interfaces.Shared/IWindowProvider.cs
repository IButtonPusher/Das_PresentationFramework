using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Mvvm;
using Das.Views.Panels;

namespace Das.Views
{
    public interface IWindowProvider<out TWindow> : IWindowNotifier<TWindow>
        where TWindow : IVisualHost
    {
        TWindow Show<TRectangle>(IView view,
                                 TRectangle rect)
            where TRectangle : IRectangle;

        TWindow Show(IView view);

        //TWindow Show<TViewModel, TRectangle>(IView view)
        //    where TRectangle : IRectangle;

        //where TViewModel : IViewModel;
    }
}
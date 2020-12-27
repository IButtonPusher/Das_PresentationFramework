using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
// ReSharper disable UnusedMember.Global

namespace Das.Views
{
    public interface IWindowProvider<out TWindow> : IWindowNotifier<TWindow>
        where TWindow : IVisualHost
    {
        TWindow Show<TRectangle>(IView view,
                                 TRectangle rect)
            where TRectangle : IRectangle;

        TWindow Show(IView view);
    }
}
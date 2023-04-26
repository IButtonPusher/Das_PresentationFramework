using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

// ReSharper disable UnusedMember.Global

namespace Das.Views;

public interface IWindowProvider<out TWindow> : IWindowNotifier<TWindow>
   where TWindow : IVisualHost
{
   TWindow Show<TRectangle>(IVisualElement view,
                            TRectangle rect)
      where TRectangle : IRectangle;

   TWindow Show(IVisualElement visual);
}
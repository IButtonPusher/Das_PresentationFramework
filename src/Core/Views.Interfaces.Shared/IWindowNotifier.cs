using System;
using System.Threading.Tasks;

namespace Das.Views;

public interface IWindowNotifier<out TWindow>
   where TWindow : IVisualHost
{
   event Action<TWindow>? WindowShown;
}
using System;
using System.Threading.Tasks;

namespace Das.Views.Input;

public interface IInputContext : IInputProvider,
                                 IMouseCaptureManager
{
   Boolean IsMousePresent { get; }

   Double ZoomLevel { get; }
}
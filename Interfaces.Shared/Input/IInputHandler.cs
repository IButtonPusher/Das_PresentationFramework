using System;
using System.Threading.Tasks;

namespace Das.Views.Input
{
    public interface IInputHandler : IDisposable, 
                                     IMouseInputHandler
    {
        void OnKeyboardStateChanged();
    }
}
using System;

namespace Das.Views.Winforms
{
    public interface IWindowsHost : IHost
    {
        IntPtr Handle { get; }
    }
}

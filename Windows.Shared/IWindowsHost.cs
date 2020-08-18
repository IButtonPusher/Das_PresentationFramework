using System;
using System.Threading.Tasks;

namespace Das.Views.Windows
{
    public interface IWindowsHost : IHost
    {
        IntPtr Handle { get; }

        Task<IntPtr> GraphicsDeviceContextPromise { get; }
    }
}
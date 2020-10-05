using System;
using System.Threading.Tasks;

namespace Das.Views.Windows
{
    public interface IWindowsHost : IHost
    {
        Task<IntPtr> GraphicsDeviceContextPromise { get; }

        IntPtr Handle { get; }
    }
}
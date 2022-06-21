using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Das.Views.Resources
{
    /// <summary>
    ///     ResourceHandle currently encapsulates an unmanaged resource handle.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct ResourceHandle
    {
        public static readonly ResourceHandle Null = new ResourceHandle(0);

        public static explicit operator UInt32(ResourceHandle r)
        {
            return r._handle;
        }

        [FieldOffset(0)]
        private readonly UInt32 _handle;

        public ResourceHandle(UInt32 handle)
        {
            _handle = handle;
        }

        /// <summary>
        ///     Checks if the handle is null.
        /// </summary>
        public Boolean IsNull => _handle == 0;
    }
}

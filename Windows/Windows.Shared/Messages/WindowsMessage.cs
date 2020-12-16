using System;
using System.Threading.Tasks;

namespace Windows.Shared
{
    public class WindowsMessage
    {
        public IntPtr LParam { get; set; }

        public MessageTypes MessageType { get; set; }

        public IntPtr WParam { get; set; }
    }
}
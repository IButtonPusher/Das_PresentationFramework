using System;

namespace Windows.Shared
{
    public class WindowsMessage
    {
        public MessageTypes MessageType { get; set; }

        public IntPtr WParam { get; set; }

        public IntPtr LParam { get; set; }
    }
}

using Das.Views.Core.Drawing;
using Das.Views.Extended.Core;

namespace Das.Views.Extended.Runtime
{
    public interface IFollowCamera<out TFrame> : ICamera<TFrame>
        where TFrame : IFrame
    {
        IPoint3d Target { get; }
    }
}

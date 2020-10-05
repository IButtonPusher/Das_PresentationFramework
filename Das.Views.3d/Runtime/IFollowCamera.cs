using Das.Views.Core.Drawing;

namespace Das.Views.Extended.Runtime
{
    public interface IFollowCamera<out TFrame> : ICamera<TFrame>
        where TFrame : IFrame
    {
        IPoint3D Target { get; }
    }
}

using System;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views.Input
{
    public interface IInputContext : IInputProvider
    {
        Boolean IsMousePresent { get; }

        Double MaximumFlingVelocity { get; }

        Double MinimumFlingVelocity { get; }

        Boolean TryCaptureMouseInput(IVisualElement view);

        Boolean TryReleaseMouseCapture(IVisualElement view);
    }
}
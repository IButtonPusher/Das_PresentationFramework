using System;
using Das.Views.Rendering;

namespace Das.Views.Input
{
    public interface IMouseCaptureManager
    {
        Boolean TryCaptureMouseInput(IVisualElement view);

        Boolean TryReleaseMouseCapture(IVisualElement view);

        IVisualElement? GetVisualWithMouseCapture();
    }
}

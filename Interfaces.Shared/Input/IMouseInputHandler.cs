using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Views.Input
{
    public interface IMouseInputHandler
    {
        Boolean OnMouseInput<TArgs>(TArgs args,
                                    InputAction action)
            where TArgs : IMouseInputEventArgs<TArgs>;

        /// <summary>
        ///     Special case since it happens so frequently
        /// </summary>
        /// <param name="position">Position relative to the main window</param>
        /// <param name="inputContext"></param>
        Boolean OnMouseMove<TPoint>(TPoint position,
                                    IInputContext inputContext)
            where TPoint : IPoint2D;

        Boolean TryCaptureMouseInput(IVisualElement view);

        Boolean TryReleaseMouseCapture(IVisualElement view);
    }
}
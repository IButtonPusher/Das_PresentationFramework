using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Views.Input
{
    public interface IElementLocator
    {
        IEnumerable<IRenderedVisual> GetElementsAt<TPoint>(TPoint point2D)
            where TPoint : IPoint2D;

        IEnumerable<IRenderedVisual<IHandleInput<T>>> GetRenderedVisualsForMouseInput<T, TPoint>(
            TPoint point2D,
            InputAction inputAction)
            where T : IInputEventArgs
            where TPoint : IPoint2D;

        IEnumerable<TVisual> GetVisualsForInput<TVisual, TPoint>(TPoint point2D,
                                                                 InputAction inputAction)
            where TVisual : class
            where TPoint : IPoint2D;

        IEnumerable<IHandleInput<T>> GetVisualsForMouseInput<T, TPoint>(TPoint point2D,
                                                                        InputAction inputAction)
            where T : IInputEventArgs
            where TPoint : IPoint2D;


        ICube? TryGetElementBounds(IVisualElement element);

        ICube? TryGetLastRenderBounds(IVisualElement element);
    }
}
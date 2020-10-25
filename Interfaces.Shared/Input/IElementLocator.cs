using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Views.Input
{
    public interface IElementLocator
    {
        IEnumerable<IRenderedVisual> GetElementsAt(IPoint2D point2D);

        IEnumerable<T> GetVisualsForInput<T>(IPoint2D point2D, 
                                           InputAction inputAction) where T : class;

        IEnumerable<IHandleInput<T>> GetVisualsForMouseInput<T>(IPoint2D point2D,
                                                                InputAction inputAction) 
            where T : IInputEventArgs;

        IEnumerable<IRenderedVisual<IHandleInput<T>>> GetRenderedVisualsForMouseInput<T>(IPoint2D point2D,
                                                                InputAction inputAction) 
            where T : IInputEventArgs;


        ICube? TryGetElementBounds(IVisualElement element);

    }
}
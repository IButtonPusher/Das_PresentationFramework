using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Views.Input
{
   public interface IElementLocator
   {
      IVisualElement? RootVisual { get; }


      IEnumerable<IRenderedVisual<IHandleInput<T>>> GetRenderedVisualsForMouseInput<T, TPoint>(TPoint point2D,
         InputAction inputAction)
         where T : IInputEventArgs
         where TPoint : IPoint2D;


      Boolean TryGetElementBounds(IVisualElement element,
                                  out ValueCube bounds);

      Boolean TryGetLastRenderBounds(IVisualElement element,
                                     out ValueCube bounds);
   }
}

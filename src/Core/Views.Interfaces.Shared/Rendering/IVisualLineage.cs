using System;
using System.Collections.Generic;

namespace Das.Views.Rendering
{
    /// <summary>
    /// The Logical hierarchy of visuals.  Does not deal with coordinates, margins, etc
    /// Allows traversal of the visual hierarchy starting from the visual
    /// currently being rendered up to the root of the visual tree
    /// </summary>
    public interface IVisualLineage : IEnumerable<IVisualElement>
    {
        void PushVisual(IVisualElement visual);

        IVisualElement PopVisual();

        /// <summary>
        /// Ensures that the provided visual is the one at the top of the stack and pops it
        /// </summary>
        /// <exception cref="InvalidOperationException">if the popped visuale is not the provided value</exception>
        void AssertPopVisual(IVisualElement visual);

        IVisualElement? PeekVisual();

        /// <summary>
        /// If the current visual on top of the stack has an IVisualContainer parent,
        /// and that parent has another child after it in it's Children collection,
        /// this method returns it
        /// </summary>
        /// <returns></returns>
        IVisualElement? GetNextSibling();
        
        Int32 Count { get; }
    }
}

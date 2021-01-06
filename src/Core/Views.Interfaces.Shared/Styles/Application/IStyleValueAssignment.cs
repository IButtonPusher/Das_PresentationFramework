using System;

namespace Das.Views.Styles.Application
{
    /// <summary>
    /// The assignment of a single value to a dependency property on one or more visuals
    /// </summary>
    public interface IStyleValueAssignment<out TVisual, out TValue> : IStyleValueAssignment
        where TVisual : IVisualElement
    {
        //IDependencyProperty<TVisual, TValue> DependencyProperty { get; }

        //IEnumerable<TVisual> Visuals { get; }

        TValue Value { get; }

        new TVisual Visual { get; }
    }

    public interface IStyleValueAssignment : IStyleApplication
    {
        /// <summary>
        /// Checks if the assignments are to the same property
        /// </summary>
        Boolean DoOverlap(IStyleValueAssignment other);

        IVisualElement Visual { get; }
    }
}

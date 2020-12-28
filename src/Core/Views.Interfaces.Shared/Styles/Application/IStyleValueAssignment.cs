using System;
using System.Collections.Generic;

namespace Das.Views.Styles.Application
{
    /// <summary>
    /// The assignment of a single value to a dependency property on one or more visuals
    /// </summary>
    public interface IStyleValueAssignment<TVisual, TValue> : IStyleValueAssignment
        where TVisual : IVisualElement
    {
        IDependencyProperty<TVisual, TValue> DependencyProperty { get; }

        IEnumerable<TVisual> Visuals { get; }

        TValue Value { get; }
    }

    public interface IStyleValueAssignment : IStyleApplication
    {
    }
}

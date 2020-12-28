using System;

namespace Das.Views.Styles.Application
{
    /// <summary>
    /// A filter on a dependency property value of a specific visual
    /// </summary>
    public interface IStyleCondition<TVisual, TValue> : IStyleCondition
        where TVisual : IVisualElement
    {
        IDependencyProperty<TVisual, TValue> DependencyProperty { get; }

        TVisual Visual { get; }

        TValue Value { get; }
    }


    public interface IStyleCondition
    {
        Boolean CanExecute();
    }
}

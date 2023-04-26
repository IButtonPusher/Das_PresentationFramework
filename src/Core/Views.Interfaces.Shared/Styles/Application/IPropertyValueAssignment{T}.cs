using System;

namespace Das.Views.Styles.Application;

public interface IPropertyValueAssignment<TVisual, T> : IStyleValueAssignment<TVisual, T>
   where TVisual : IVisualElement
{
   IDependencyProperty<TVisual, T> Property { get; }
}
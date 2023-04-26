using System;
using Das.Views.DependencyProperties;

namespace Das.Views.Input;

public static class InteractiveVisualProperties
{
   public static readonly DependencyProperty<IInteractiveVisual, Boolean> IsActiveProperty =
      DependencyProperty<IInteractiveVisual, Boolean>.Register(
         "IsActive", false, PropertyMetadata.AffectsArrange);

   public static readonly DependencyProperty<IInteractiveVisual, Boolean> IsFocusedProperty =
      DependencyProperty<IInteractiveVisual, Boolean>.Register(
         "IsFocused", false);

   public static readonly DependencyProperty<IInteractiveVisual, Boolean> IsMouseOverProperty =
      DependencyProperty<IInteractiveVisual, Boolean>.Register(
         "IsMouseOver", false);

}
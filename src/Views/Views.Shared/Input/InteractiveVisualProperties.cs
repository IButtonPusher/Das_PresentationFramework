using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Input
{
    public static class InteractiveVisualProperties
    {
        public static readonly DependencyProperty<IInteractiveVisual, Boolean> IsActiveProperty =
            DependencyProperty<IInteractiveVisual, Boolean>.Register(
                "IsActive", false);

        public static readonly DependencyProperty<IInteractiveVisual, Boolean> IsFocusedProperty =
            DependencyProperty<IInteractiveVisual, Boolean>.Register(
                "IsFocused", false);

        public static readonly DependencyProperty<IInteractiveVisual, Boolean> IsMouseOverProperty =
            DependencyProperty<IInteractiveVisual, Boolean>.Register(
                "IsMouseOver", false);

    }
}

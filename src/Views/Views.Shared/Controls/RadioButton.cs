using System;
using Das.Views.Input;

namespace Das.Views.Controls;

public class RadioButton : ToggleButton
{
   public RadioButton(IVisualBootstrapper templateResolver) : base(templateResolver)
   {
   }

   public override Boolean OnInput(MouseClickEventArgs args)
   {
      if (IsChecked == true)
         return false;

      return base.OnInput(args);
   }
}

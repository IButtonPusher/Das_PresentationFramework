using Das.Views.Input;
using System;
using Das.ViewModels;

namespace Das.Views.Controls
{
    public class ButtonBase : ButtonBase<Object>
    {
        public override Boolean OnInput(MouseClickEventArgs args)
        {
            if (ClickMode != ClickMode.Release || !(Command is {} cmd))
                return true;

            cmd.ExecuteAsync().ConfigureAwait(false);
            return true;
        }

        public new IObservableCommand? Command { get; set; }

        public ButtonBase(IVisualBootStrapper templateResolver) : base(templateResolver)
        {
        }
    }
}

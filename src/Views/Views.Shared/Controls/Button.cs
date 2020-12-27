using System;
using System.Threading.Tasks;
using Das.Views.Input;

namespace Das.Views.Controls
{
    public class Button : ButtonBase,
                          IButton
    {
        public Button(IVisualBootstrapper templateResolver) : base(templateResolver)
        {
        }

        public override InputVisualType InputType => InputVisualType.Button;
    }
}
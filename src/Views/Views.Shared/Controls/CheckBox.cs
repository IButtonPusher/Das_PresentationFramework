using System;

namespace Das.Views.Controls
{
    public class CheckBox : ToggleButton
    {
        protected override void OnDataContextChanged(Object? newValue)
        {
            base.OnDataContextChanged(newValue);
        }

        public CheckBox(IVisualBootstrapper templateResolver) 
            : base(templateResolver)
        {
        }
    }
}

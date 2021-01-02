using System;
using Das.Views.Core.Geometry;
using Das.Views.Input;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Controls
{
    // ReSharper disable once UnusedType.Global
    public class ToggleButton : ButtonBase,
                                IToggleButton
    {
        public ToggleButton(IVisualBootstrapper templateResolver)
            : base(templateResolver)
        {
            IsChecked = false;
        }

        public override Boolean OnInput(MouseClickEventArgs args)
        {
            base.OnInput(args);

            IsChecked = !IsChecked;
            return true;
        }

        public override InputVisualType InputType => InputVisualType.CheckBox;

        public static readonly DependencyProperty<IToggleButton, Boolean?> IsCheckedProperty =
            DependencyProperty<IToggleButton, Boolean?>.Register(
                nameof(IsChecked), false);

        public override ValueSize Measure(IRenderSize availableSpace, IMeasureContext measureContext)
        {
            var res = base.Measure(availableSpace, measureContext);

            if (Style != null)
            {}

            return res;
        }

        public Boolean? IsChecked
        {
            get => IsCheckedProperty.GetValue(this);
            set => IsCheckedProperty.SetValue(this, value, OnIsCheckedChanged);
        }

        private void OnIsCheckedChanged(Boolean? oldValue,
                                        Boolean? value)
        {
            switch (value)
            {
                case true:
                    AddStyleSelector(VisualStateType.Checked);
                    break;

                case false:
                    RemoveStyleSelector(VisualStateType.Checked);
                    break;
            }

        }
    }
}

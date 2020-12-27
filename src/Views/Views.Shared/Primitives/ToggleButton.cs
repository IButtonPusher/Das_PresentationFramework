﻿using System;
using Das.Views.Input;
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

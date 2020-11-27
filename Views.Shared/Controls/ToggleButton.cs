using System;
using System.Diagnostics;
using Das.Views.Input;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Controls
{
    // ReSharper disable once UnusedType.Global
    public class ToggleButton<T> : ButtonBase<T>,
                                   IToggleButton
    {
        public ToggleButton(IVisualBootStrapper templateResolver)
            : base(templateResolver)
        {
            IsChecked = false;
        }

        //public override void Arrange(IRenderSize availableSpace, IRenderContext renderContext)
        //{
        //    base.Arrange(availableSpace, renderContext);
        //}

        public override Boolean OnInput(MouseClickEventArgs args)
        {
            base.OnInput(args);

            IsChecked = !IsChecked;
            return true;
        }

        private Boolean? _isChecked;

        public Boolean? IsChecked
        {
            get => _isChecked;
            set => SetValue(ref _isChecked, value, OnIsCheckedChanged);
        }

        private void OnIsCheckedChanged(Boolean? value)
        {
            switch (value)
            {
                case true:
                    AddStyleSelector(StyleSelector.Checked);
                    break;

                case false:
                    RemoveStyleSelector(StyleSelector.Checked);
                    break;
            }

            
        }
    }
}

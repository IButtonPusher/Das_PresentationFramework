using System;
using System.Threading.Tasks;
using Das.ViewModels;
using Das.Views.Core.Geometry;
using Das.Views.Input;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Controls
{
    public class ButtonBase<T> : ContentPanel<T>,
                                 IHandleInput<MouseClickEventArgs>,
                                 IHandleInput<MouseDownEventArgs>,
                                 IHandleInput<MouseUpEventArgs>,
                                 IHandleInput<MouseOverEventArgs>,
                                 IButtonBase
    {
        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
        }

        public override void Dispose()
        {
        }

        public override ISize Measure(IRenderSize availableSpace,
                                      IMeasureContext measureContext)
        {
            return Size.Empty;
        }

        public StyleSelector CurrentStyleSelector
        {
            get => _currentStyleSelector;
            set => SetValue(ref _currentStyleSelector, value, OnCurrentSelectorChanged);
        }

        public virtual Boolean OnInput(MouseClickEventArgs args)
        {
            if (ClickMode != ClickMode.Release || !(Command is {} cmd))
                return true;

            cmd.ExecuteAsync(BoundValue).ConfigureAwait(false);
            return true;
        }

        InputAction IInteractiveView.HandlesActions => InputAction.LeftMouseButtonDown |
                                                       InputAction.LeftMouseButtonUp |
                                                       InputAction.LeftClick |
                                                       InputAction.MouseOver;

        public virtual Boolean OnInput(MouseDownEventArgs args)
        {
            CurrentStyleSelector = StyleSelector.Active;

            if (ClickMode != ClickMode.Press || !(Command is {} cmd))
                return true;

            cmd.ExecuteAsync(BoundValue).ConfigureAwait(false);
            return true;
        }

        public virtual Boolean OnInput(MouseOverEventArgs args)
        {
            CurrentStyleSelector = args.IsMouseOver
                ? StyleSelector.Hover
                : StyleSelector.None;

            return true;
        }

        public virtual Boolean OnInput(MouseUpEventArgs args)
        {
            CurrentStyleSelector = StyleSelector.Hover;
            return true;
        }

        public ClickMode ClickMode
        {
            get => _clickMode;
            set => SetValue(ref _clickMode, value);
        }

        public IObservableCommand<T>? Command { get; set; }

        private void OnCurrentSelectorChanged(StyleSelector value)
        {
            IsChanged = true;
        }


        private ClickMode _clickMode;


        private StyleSelector _currentStyleSelector;
    }
}
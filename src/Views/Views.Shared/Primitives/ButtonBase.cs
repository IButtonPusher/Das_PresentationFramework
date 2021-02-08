using System;
using System.Threading.Tasks;
using Das.ViewModels;
using Das.Views.Core.Geometry;
using Das.Views.Input;
using Das.Views.Panels;
using Das.Views.Rendering;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Das.Views.Controls
{
    public abstract class ButtonBase : ContentPanel,
                                       IHandleInput<MouseClickEventArgs>,
                                       IHandleInput<MouseDownEventArgs>,
                                       IHandleInput<MouseUpEventArgs>,
                                       IHandleInput<MouseOverEventArgs>,
                                       IHandleInput<LostCaptureEventArgs>,
                                       IButtonBase
    {
        protected ButtonBase(IVisualBootstrapper visualBootstrapper)
            : base(visualBootstrapper)
        {
            _lastRenderSize = Size.Empty;
        }

        public override void Arrange<TRenderSize>(TRenderSize availableSpace,
                                                 IRenderContext renderContext)
        {
            _lastRenderSize = availableSpace;
            base.Arrange(availableSpace, renderContext);
        }

        public Boolean IsActive
        {
            get => InteractiveVisualProperties.IsActiveProperty.GetValue(this);
            set => InteractiveVisualProperties.IsActiveProperty.SetValue(this, value);
        }

        public Boolean IsFocused
        {
            get => InteractiveVisualProperties.IsFocusedProperty.GetValue(this);
            set => InteractiveVisualProperties.IsFocusedProperty.SetValue(this, value);
        }

        public Boolean IsMouseOver
        {
            get => InteractiveVisualProperties.IsMouseOverProperty.GetValue(this);
            set => InteractiveVisualProperties.IsMouseOverProperty.SetValue(this, value);
        }

        public IObservableCommand? Command { get; set; }

        public abstract InputVisualType InputType { get; }

        public virtual Boolean OnInput(MouseClickEventArgs args)
        {
            ClickToAction(ClickMode.Release);
            return true;
        }

        InputAction IHandleInput.HandlesActions => I_HANDLE_INPUT;

        public virtual Boolean OnInput(MouseDownEventArgs args)
        {
            if (args.InputContext.TryCaptureMouseInput(this))
                IsActive = true;

            ClickToAction(ClickMode.Press);
            return true;
        }

        public virtual Boolean OnInput(MouseOverEventArgs args)
        {
            IsMouseOver = args.IsMouseOver;

            return true;
        }

        public virtual Boolean OnInput(MouseUpEventArgs args)
        {
            args.InputContext.TryReleaseMouseCapture(this);
            IsActive = false;

            if (args.PositionWentDown != null && Math.Abs(args.PositionWentDown.X -
                                                          args.Position.X) > _lastRenderSize.Width)
                return false;

            return true;
        }

        public ClickMode ClickMode
        {
            get => _clickMode;
            set => SetValue(ref _clickMode, value);
        }


        public Object? CommandParameter { get; set; }


        public Boolean OnInput(LostCaptureEventArgs args)
        {
            IsActive = false;
            return true;
        }

        public override String ToString()
        {
            return base.ToString() + " - " + DataContext;
        }

        private void ClickToAction(ClickMode clickType)
        {
            if (ClickMode != clickType || !(Command is { } cmd))
                return;

            if (CommandParameter is { } validParameter)
                cmd.ExecuteAsync(validParameter).ConfigureAwait(false);
            else if (DataContext is { } boundValue)
                cmd.ExecuteAsync(boundValue).ConfigureAwait(false);
            else
                cmd.ExecuteAsync().ConfigureAwait(false);
        }

        private const InputAction I_HANDLE_INPUT = InputAction.LeftMouseButtonDown |
                                                   InputAction.LeftMouseButtonUp |
                                                   InputAction.LeftClick |
                                                   InputAction.MouseOver | 
                                                   InputAction.LostCapture;

        private ClickMode _clickMode;
        private ISize _lastRenderSize;
    }
}

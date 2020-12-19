using Das.Views.Input;
using System;
using Das.ViewModels;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Controls
{
    public abstract class ButtonBase : ContentPanel,
                                       IHandleInput<MouseClickEventArgs>,
                                       IHandleInput<MouseDownEventArgs>,
                                       IHandleInput<MouseUpEventArgs>,
                                       IHandleInput<MouseOverEventArgs>,
                                       IButtonBase
    {
        private ISize _lastRenderSize;

        protected ButtonBase(IVisualBootstrapper visualBootstrapper)
        : base(visualBootstrapper)
        {
            _currentStyleSelector = StyleSelector.None;
            _lastRenderSize = Size.Empty;
        }

        protected override Thickness? GetPadding(IStyleProvider styleContext)
        {
            return styleContext.GetStyleSetter<Thickness>(StyleSetterType.Padding,
                CurrentStyleSelector, this);
        }

        public StyleSelector CurrentStyleSelector
        {
            get => _currentStyleSelector;
        }

        protected void AddStyleSelector(StyleSelector value)
        {
            var val = _currentStyleSelector == StyleSelector.None 
            ? value : 
            _currentStyleSelector | value;

            if (SetValue(ref _currentStyleSelector, val, OnCurrentSelectorChanged,
                nameof(CurrentStyleSelector)))
            {
                InvalidateArrange();
            }
        }

        protected void RemoveStyleSelector(StyleSelector value)
        {
            var val = _currentStyleSelector & ~value;

            if (val == 0)
            {
                val = StyleSelector.None;
            }

            SetValue(ref _currentStyleSelector, val, OnCurrentSelectorChanged,
                nameof(CurrentStyleSelector));
        }

        public virtual Boolean OnInput(MouseClickEventArgs args)
        {
            if (ClickMode != ClickMode.Release || !(Command is {} cmd))
                return true;

            //todo: this is not good
            //var boundValue = BoundValue ?? Binding?.GetBoundValue(DataContext);
            var boundValue = DataContext;

            cmd.ExecuteAsync(boundValue!).ConfigureAwait(false);
            return true;
        }

        InputAction IInteractiveView.HandlesActions => I_HANDLE_INPUT;

        private const InputAction I_HANDLE_INPUT = InputAction.LeftMouseButtonDown |
                                             InputAction.LeftMouseButtonUp |
                                             InputAction.LeftClick |
                                             InputAction.MouseOver;

        public virtual Boolean OnInput(MouseDownEventArgs args)
        {
            args.InputContext.TryCaptureMouseInput(this);

            AddStyleSelector(StyleSelector.Active);

            if (ClickMode != ClickMode.Press || !(Command is {} cmd))
                return true;

            //var boundValue = DataContext;
            
            if (DataContext is {} boundValue)
                cmd.ExecuteAsync(boundValue).ConfigureAwait(false);
            else
                cmd.ExecuteAsync().ConfigureAwait(false);
            return true;
        }

        public virtual Boolean OnInput(MouseOverEventArgs args)
        {
            if (args.IsMouseOver)
                AddStyleSelector(StyleSelector.Hover);
            else
                RemoveStyleSelector(StyleSelector.Hover);

            return true;
        }

        public override void Arrange(IRenderSize availableSpace, 
                                     IRenderContext renderContext)
        {
            _lastRenderSize = availableSpace;
            base.Arrange(availableSpace, renderContext);
        }

        public virtual Boolean OnInput(MouseUpEventArgs args)
        {
            RemoveStyleSelector(StyleSelector.Active);
            args.InputContext.TryReleaseMouseCapture(this);

            if (args.PositionWentDown != null && Math.Abs(args.PositionWentDown.X -
                                                          args.Position.X) > _lastRenderSize.Width)
            {
                return false;
            }
            
            return true;
        }

        public ClickMode ClickMode
        {
            get => _clickMode;
            set => SetValue(ref _clickMode, value);
        }

        public IObservableCommand? Command { get; set; }

        private void OnCurrentSelectorChanged(StyleSelector value)
        {
            InvalidateArrange();
        }

        public override String ToString()
        {
            return base.ToString() + " - " + DataContext;
        }


        private ClickMode _clickMode;
        private StyleSelector _currentStyleSelector;
    }
}

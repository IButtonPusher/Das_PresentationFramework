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
            _currentVisualStateType = VisualStateType.None;
            _lastRenderSize = Size.Empty;
        }

        protected override Thickness? GetPadding(IStyleProvider styleContext)
        {
            return styleContext.GetStyleSetter<Thickness>(StyleSetterType.Padding,
                CurrentVisualStateType, this);
        }

        public VisualStateType CurrentVisualStateType
        {
            get => _currentVisualStateType;
        }

        protected void AddStyleSelector(VisualStateType value)
        {
            var val = _currentVisualStateType == VisualStateType.None 
            ? value : 
            _currentVisualStateType | value;

            if (SetValue(ref _currentVisualStateType, val, OnCurrentSelectorChanged,
                nameof(CurrentVisualStateType)))
            {
                InvalidateArrange();
            }
        }

        protected void RemoveStyleSelector(VisualStateType value)
        {
            var val = _currentVisualStateType & ~value;

            if (val == 0)
            {
                val = VisualStateType.None;
            }

            SetValue(ref _currentVisualStateType, val, OnCurrentSelectorChanged,
                nameof(CurrentVisualStateType));
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

            AddStyleSelector(VisualStateType.Active);

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
                AddStyleSelector(VisualStateType.Hover);
            else
                RemoveStyleSelector(VisualStateType.Hover);

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
            RemoveStyleSelector(VisualStateType.Active);
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

        private void OnCurrentSelectorChanged(VisualStateType value)
        {
            InvalidateArrange();
        }

        public override String ToString()
        {
            return base.ToString() + " - " + DataContext;
        }


        private ClickMode _clickMode;
        private VisualStateType _currentVisualStateType;

        public abstract InputVisualType InputType { get; }
    }
}

using Das.Views.Input;
using System;
using Das.ViewModels;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

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

        //protected override Thickness? GetPadding(IStyleProvider styleContext)
        //{
        //    return styleContext.GetStyleSetter<Thickness>(StyleSetterType.Padding,
        //        CurrentVisualStateType, this);
        //}

        public VisualStateType CurrentVisualStateType
        {
            get => _currentVisualStateType;
        }

        protected void AddStyleSelector(VisualStateType value)
        {
            var val = _currentVisualStateType == VisualStateType.None 
            ? value : 
            _currentVisualStateType | value;

            SetValue(ref _currentVisualStateType, val, OnCurrentSelectorChanged,
                nameof(CurrentVisualStateType));

            //if (SetValue(ref _currentVisualStateType, val, OnCurrentSelectorChanged,
            //    nameof(CurrentVisualStateType)))
            //{
            //    InvalidateArrange();
            //}
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
            ClickToAction(ClickMode.Release);
            return true;
        }

        private void ClickToAction(ClickMode clickType)
        {
            if (ClickMode != clickType || !(Command is {} cmd))
                return;

            if (DataContext is {} boundValue)
                cmd.ExecuteAsync(boundValue).ConfigureAwait(false);
            else
                cmd.ExecuteAsync().ConfigureAwait(false);
        }

        InputAction IHandleInput.HandlesActions => I_HANDLE_INPUT;

        private const InputAction I_HANDLE_INPUT = InputAction.LeftMouseButtonDown |
                                             InputAction.LeftMouseButtonUp |
                                             InputAction.LeftClick |
                                             InputAction.MouseOver;

        public virtual Boolean OnInput(MouseDownEventArgs args)
        {
            args.InputContext.TryCaptureMouseInput(this);

            AddStyleSelector(VisualStateType.Active);

            ClickToAction(ClickMode.Press);
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

        protected virtual void OnCurrentSelectorChanged(VisualStateType value)
        {
            //InvalidateArrange();
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

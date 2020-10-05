using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Input;
using Das.Views.Rendering;
using Das.ViewModels;

namespace Das.Views.Controls
{
    public class ButtonBase<T> : BindableElement<T>,
                                 IHandleInput<MouseClickEventArgs>,
                                 IHandleInput<MouseDownEventArgs>,
                                 IHandleInput<MouseUpEventArgs>,
                                 IHandleInput<MouseOverEventArgs>
    {
        public ButtonBase()
        {
        
        }

        public virtual Boolean OnInput(MouseClickEventArgs args)
        {
            if (ClickMode != ClickMode.Release || !(Command is {} cmd))
                return true;

            cmd.ExecuteAsync(BoundValue).ConfigureAwait(false);
            return true;
        }

        public virtual Boolean OnInput(MouseDownEventArgs args)
        {
            if (ClickMode != ClickMode.Press || !(Command is {} cmd))
                return true;

            cmd.ExecuteAsync(BoundValue).ConfigureAwait(false);
            return true;
        }

        public virtual Boolean OnInput(MouseUpEventArgs args)
        {
            throw new NotImplementedException();
        }

        public virtual Boolean OnInput(MouseOverEventArgs args)
        {
            throw new NotImplementedException();
        }

        InputAction IInteractiveView.HandlesActions =>InputAction.MouseDown |
                                                      InputAction.MouseUp |
                                                      InputAction.LeftClick |
                                                      InputAction.MouseOver;

        public IObservableCommand<T>? Command { get; set; }


        private ClickMode _clickMode;

        public ClickMode ClickMode
        {
            get => _clickMode;
            set => SetValue(ref _clickMode, value);
        }

        public override void Arrange(ISize availableSpace, 
                                     IRenderContext renderContext)
        {
        }

        public override void Dispose()
        {
        }

        public override ISize Measure(ISize availableSpace, 
                                      IMeasureContext measureContext)
        {
            return Size.Empty;
        }
    }
}
using System;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Input;
using Das.Views.Rendering;
using Das.ViewsModels;

namespace Das.Views.Controls
{
    public class ButtonBase<T> : BindableElement<T>, IInteractiveView
    {
        public ButtonBase()
        {
            HandlesActions = InputAction.MouseDown | InputAction.MouseUp | InputAction.LeftClick;
        }

        public override ISize Measure(ISize availableSpace, IMeasureContext measureContext)
        {
            return Size.Empty;
        }

        public override void Arrange(ISize availableSpace, IRenderContext renderContext)
        {
            
        }

        public override void Dispose()
        {
            
        }

        public InputAction HandlesActions { get; }

        public IObservableCommand<T>? Command { get; set; }
    }
}

using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Input;

public readonly struct MouseDownEventArgs : IMouseButtonEventArgs<MouseDownEventArgs>
{
   public MouseDownEventArgs(IPoint2D position,
                             MouseButtons button,
                             IInputContext inputContext)
   {
      Position = position;
      Button = button;
      InputContext = inputContext;
      switch (button)
      {
         case MouseButtons.Left:
            Action = InputAction.LeftMouseButtonDown;
            break;

         case MouseButtons.Right:
            Action = InputAction.RightMouseButtonDown;
            break;

         default:
            throw new NotSupportedException();
      }
   }

   public IPoint2D Position { get; }

   public MouseButtons Button { get; }

   public MouseDownEventArgs Offset(IPoint2D position)
   {
      return new MouseDownEventArgs(Position.Offset(position),
         Button, InputContext);
   }

   public MouseDownEventArgs Offset(Double pct)
   {
      return new MouseDownEventArgs(Position.Offset(pct),
         Button, InputContext);
   }

   public InputAction Action { get; }

   public IInputContext InputContext { get; }
}
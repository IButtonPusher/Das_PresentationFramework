using Das.Views.Input;
using System;

namespace Das.Views.Panels;

public partial class ScrollPanel
{
   public virtual Boolean OnInput(DragEventArgs args)
   {
      //Debug.WriteLine("scroll Handle drag: " + args.LastChange.Height);

      if (args.InputContext.IsMousePresent && !IsScrollWithMouseDrag)
         //todo: only use drag in a touch-only scenario?
         return false;

      var isCapture = (IsScrollsHorizontal && Math.Abs(args.TotalDragged.X) > MOUSE_CAPTURE_THRESHOLD) ||
                      (IsScrollsVertical && Math.Abs(args.TotalDragged.Y) > MOUSE_CAPTURE_THRESHOLD);

      if (isCapture)
      {
         _inputContext = args.InputContext;
         _inputContext.TryCaptureMouseInput(this);
      }

      // use the opposites here because if we are dragging, for example,
      // to the right, that's a +X but we want the scroll to decrease
      var res = OnScroll(0 - args.LastChange.Width,
         0 - args.LastChange.Height);

      return res;
   }

   public Boolean OnInput(FlingEventArgs args)
   {
      if (!IsScrollWithMouseDrag)
         return false;

      var working = _flingHandler.OnInput(args);

      if (working)
         args.InputContext.TryCaptureMouseInput(this);

      return working;
   }


   public Boolean OnInput(MouseDownEventArgs args) => IsScrollWithMouseDrag && _flingHandler.OnInput(args);

   public Boolean OnInput(MouseUpEventArgs args)
   {
      if (args.PositionWentDown == null ||
          args.InputContext.GetVisualWithMouseCapture() != this)
         return false;


      args.InputContext.TryReleaseMouseCapture(this);

      if (IsScrollsHorizontal)
      {
         var diffX = args.PositionWentDown.X - args.Position.X;
         if (Math.Abs(diffX) >= MOUSE_UP_MOVE_THRESHOLD)
            return true;
      }

      if (IsScrollsVertical)
      {
         var diffY = args.PositionWentDown.Y - args.Position.Y;
         if (Math.Abs(diffY) >= MOUSE_UP_MOVE_THRESHOLD)
            return true;
      }


      return false;
   }

   public InputAction HandlesActions => InputAction.MouseDrag |
                                        InputAction.MouseWheel |
                                        InputAction.Fling;

   public Boolean OnInput(MouseWheelEventArgs args)
   {
      OnScroll(0, args.Delta * _scrollCoefficient);
      return true;
   }
}
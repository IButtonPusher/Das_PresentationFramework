using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Shared;
using Windows.Shared.Input;
using Das.Extensions;
using Das.Views.Core.Geometry;
using Das.Views.Input;
using Gdi.Shared;
using DragEventArgs = Das.Views.Input.DragEventArgs;
using MouseButtons = Das.Views.Input.MouseButtons;

namespace Das.Gdi
{
   public class InputContext : Win32InputContext,
                               IMessageFilter
   {
      public InputContext(IPositionOffseter offsetter,
                          IInputHandler inputHandler,
                          Control window,
                          IntPtr windowHandle)
         : base(offsetter, inputHandler, windowHandle)
      {
         _window = window;
         var dpi = 1.0f;
         _dpiRatio = dpi;
         var ppi = dpi * 160.0f;

         var _scrollFriction = 0.015f;
      
      var _physicalCoefficient = 9.80665f // g (m/s^2)
                                 * 39.37f // inch/meter
                                 * ppi
                                 * 0.84f; // look and feel tuning

      _flingBuilder = new FlingBuilder(dpi, _scrollFriction, _physicalCoefficient);

         Application.AddMessageFilter(this);
      }


      public Boolean PreFilterMessage(ref Message m)
      {
         ValuePoint2D pos;


         switch ((MessageTypes)m.Msg)
         {
            case MessageTypes.WM_LBUTTONDOWN:

               //pos = GetPosition(m);
               if (!TryGetPosition(m, out pos))
                  break;


               _leftButtonWentDown = pos;

               //System.Diagnostics.Debug.WriteLine("on l button down");
               _inputHandler.OnMouseInput(
                  new MouseDownEventArgs(pos, MouseButtons.Left, this),
                  InputAction.LeftMouseButtonDown);
               break;

            case MessageTypes.WM_RBUTTONDOWN:
               if (!TryGetPosition(m, out pos))
                  break;
               //pos = GetPosition(m);
               _rightButtonWentDown = pos;
               _inputHandler.OnMouseInput(
                  new MouseDownEventArgs(pos, MouseButtons.Right, this),
                  InputAction.RightMouseButtonDown);
               break;


            case MessageTypes.WM_RBUTTONUP:

               if (!TryGetPosition(m, out pos))
                  break;

               _inputHandler.OnMouseInput(
                  new MouseUpEventArgs(pos,
                     _rightButtonWentDown,
                     MouseButtons.Right, this, true),
                  InputAction.RightMouseButtonUp);
               _rightButtonWentDown = default;
               _lastDragPosition = default;
               break;

            case MessageTypes.WM_MOUSEWHEEL:

               var args = new MouseWheelEventArgs(CursorPosition,
                  (Int64)m.WParam >= Int32.MaxValue ? 1 : -1, this);

               _inputHandler.OnMouseInput(args, InputAction.MouseWheel);

               break;


            case MessageTypes.WM_LBUTTONUP:

               if (!TryGetPosition(m, out pos))
                  break;

               var dt = _lastDragTimestamp - _nextToLastDragTimestamp;

               FlingEventArgs? flingArgs = null;

               if (_lastDragPosition != null &&
                   _nextToLastDragPosition != null &&
                   dt > 0)
               {
                  var ddt = dt / 100000.0;

                  var vx = (_lastDragPosition.Value.X -
                            _nextToLastDragPosition.Value.X) / ddt;
                  var vy = (_nextToLastDragPosition.Value.Y -
                            _lastDragPosition.Value.Y) / ddt;

                  if (vx.IsNotZero() || vy.IsNotZero())
                  {
                     vx *= 50;
                     vy *= 50;

                     if (Math.Abs(vx) >= MinimumFlingVelocity ||
                         Math.Abs(vy) >= MinimumFlingVelocity)
                     {
                        _flingBuilder.BuildFlingValues(vx, out var flungX, out var xDuration);
                        _flingBuilder.BuildFlingValues(vy, out var flungY, out var yDuration);

                        flingArgs = new FlingEventArgs(vx * _dpiRatio,
                           vy* _dpiRatio, pos, this, 
                           flungX, flungY, xDuration, yDuration);

                        //flingArgs = new FlingEventArgs(vx, vy, pos, this,
                        //   0.5f, 0.5f, TimeSpan.Zero, TimeSpan.Zero); //todo:
                     }
                  }
               }

               var lBtnArgs = new MouseUpEventArgs(pos,
                  _leftButtonWentDown,
                  MouseButtons.Left, this, true);

               _leftButtonWentDown = default;
               _lastDragPosition = default;
               _nextToLastDragPosition = default;

               _nextToLastDragTimestamp = 0;
               _lastDragTimestamp = 0;

               //System.Diagnostics.Debug.WriteLine("on l button up");
               _inputHandler.OnMouseInput(lBtnArgs,
                  InputAction.LeftMouseButtonUp);

               if (flingArgs != null)
               {
                  _inputHandler.OnMouseInput(flingArgs.Value, InputAction.Fling);
               }

               break;


            case MessageTypes.WM_MOUSEMOVE:

               pos = GetPosition(m.LParam);

               //shouldn't be needed but stops debugging headaches for now
               //if (_leftButtonWentDown != null && 
               //    !IsButtonPressed(MouseButtons.Left))
               //    _leftButtonWentDown = default;
               //if (_rightButtonWentDown != null &&
               //    !IsButtonPressed(MouseButtons.Right))
               _rightButtonWentDown = default;

               var letsUse = _leftButtonWentDown ?? _rightButtonWentDown;
               if (letsUse == null)
               {
                  _inputHandler.OnMouseMove(pos, this);
                  break;
               }

               ValueSize lastDragChange;
               if (_lastDragPosition == null)
                  lastDragChange = new ValueSize(pos.X - letsUse.Value.X,
                     pos.Y - letsUse.Value.Y);
               else
                  lastDragChange = new ValueSize(pos.X - _lastDragPosition.Value.X,
                     pos.Y - _lastDragPosition.Value.Y);

               if (lastDragChange.IsEmpty)
                  break;

               _nextToLastDragPosition = _lastDragPosition;
               _lastDragPosition = pos;

               _nextToLastDragTimestamp = _lastDragTimestamp;
               _lastDragTimestamp = DateTime.Now.Ticks;

               //if (_nextToLastDragPosition != null)
               //{
               //    Debug.WriteLine("last drag: " + _lastDragPosition + " t: " + _lastDragTimestamp + 
               //                    " 2nd: " + _nextToLastDragPosition + " t: " + _nextToLastDragTimestamp);
               //}

               var dragArgs = new DragEventArgs(letsUse, pos, lastDragChange,
                  _leftButtonWentDown != null ? MouseButtons.Left : MouseButtons.Right,
                  this);

               //Debug.WriteLine("Sending drag event: " + dragArgs.LastChange.Height);

               _inputHandler.OnMouseInput(dragArgs, InputAction.MouseDrag);

               break;

            case MessageTypes.WM_QUERYDRAGICON:
               break;

            case MessageTypes.WM_MOUSEHWHEEL:
               throw new NotImplementedException();
         }

         return false;
      }


      private Boolean TryGetPosition(Message msg,
                                     out ValuePoint2D pos)
      {
         if (msg.HWnd == _window.Handle)
         {
            pos = GetPosition(msg.LParam);
            return true;
         }

         var surrogate = _window.Controls.FindControl(msg.HWnd);
         if (surrogate == null)
         {
            //TODO: why is the window handle wrong here when only a single control has been set
            //pos = ValuePoint2D.Empty;
            //return false;

            pos = GetPosition(msg.LParam);
            return true;


         }

         var posInControl = GetPosition(msg.LParam);
         pos = new ValuePoint2D(surrogate.Left + posInControl.X,
            surrogate.Top + posInControl.Y);
         return true;
      }

      //private ValuePoint2D GetPosition(Message msg)
      //{
      //    if (msg.HWnd == _window.Handle)
      //        return GetPosition(msg.LParam);

      //    var surrogate = _window.Controls.FindControl(msg.HWnd);
      //    if (surrogate == null)
      //        return ValuePoint2D.Empty;

      //    var posInControl = GetPosition(msg.LParam);
      //    return new ValuePoint2D(surrogate.Left + posInControl.X,
      //        surrogate.Top + posInControl.Y);

      //}

      private static ValuePoint2D GetPosition(IntPtr lParam)
      {
         return GetPosition((Int32)lParam);
      }


      private static ValuePoint2D GetPosition(Int32 lParam)
      {
         return new ValuePoint2D((Int16)lParam,
            (Int16)(lParam >> 16)); //cast to int16 for overflow-negatives
      }

      public override Double ZoomLevel => 1.0;

      private readonly Control _window;

      private ValuePoint2D? _lastDragPosition;

      private Int64 _lastDragTimestamp;


      private ValuePoint2D? _leftButtonWentDown;
      private ValuePoint2D? _nextToLastDragPosition;
      private Int64 _nextToLastDragTimestamp;
      private ValuePoint2D? _rightButtonWentDown;

      private readonly FlingBuilder _flingBuilder;
      private readonly Single _dpiRatio;
   }
}

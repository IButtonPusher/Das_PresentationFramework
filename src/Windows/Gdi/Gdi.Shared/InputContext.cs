using System;

using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Shared;
using Windows.Shared.Input;
using Das.Extensions;
using Das.Views.Core.Geometry;
using Das.Views.Input;
using DragEventArgs = Das.Views.Input.DragEventArgs;
using MouseButtons = Das.Views.Input.MouseButtons;

namespace Das.Gdi
{
    public class InputContext : Win32InputContext, 
                                IMessageFilter
    {
        public InputContext(IPositionOffseter offsetter,
                            IInputHandler inputHandler,
                            IntPtr windowHandle)
            : base(offsetter, inputHandler, windowHandle)
        {
            Application.AddMessageFilter(this);
        }


        public Boolean PreFilterMessage(ref Message m)
        {
            ValuePoint2D pos;

            switch ((MessageTypes)m.Msg)
            {
                case MessageTypes.WM_LBUTTONDOWN:

                    pos = GetPosition(m.LParam);
                    _leftButtonWentDown = pos;

                    _inputHandler.OnMouseInput(
                        new MouseDownEventArgs(pos, MouseButtons.Left, this),
                        InputAction.LeftMouseButtonDown);
                    break;

                case MessageTypes.WM_RBUTTONDOWN:
                    pos = GetPosition(m.LParam);
                    _rightButtonWentDown = pos;
                    _inputHandler.OnMouseInput(
                        new MouseDownEventArgs(pos, MouseButtons.Right, this),
                        InputAction.RightMouseButtonDown);
                    break;

               

                case MessageTypes.WM_RBUTTONUP:
                    
                    _inputHandler.OnMouseInput(
                        new MouseUpEventArgs(GetPosition(m.LParam),
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

                    pos = GetPosition(m.LParam);

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
                                flingArgs = new FlingEventArgs(vx, vy, pos, this);
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


        private ValuePoint2D? _leftButtonWentDown;
        private ValuePoint2D? _rightButtonWentDown;
        
        private ValuePoint2D? _lastDragPosition;
        private ValuePoint2D? _nextToLastDragPosition;
        
        private Int64 _lastDragTimestamp;
        private Int64 _nextToLastDragTimestamp;
       

        private static ValuePoint2D GetPosition(IntPtr lParam) => GetPosition((Int32) lParam);
        

        private static ValuePoint2D GetPosition(Int32 lParam)
        {
            return new ValuePoint2D((Int16) lParam,
                (Int16) (lParam >> 16)); //cast to int16 for overflow-negatives
        }
    }
}
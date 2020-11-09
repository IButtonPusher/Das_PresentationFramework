using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Shared;
using Windows.Shared.Input;
using Das.Views.Core.Geometry;
using Das.Views.Input;
using DragEventArgs = Das.Views.Input.DragEventArgs;
using MouseButtons = Das.Views.Input.MouseButtons;

namespace Das.Gdi
{
    public class InputContext : Win32InputContext, IMessageFilter
    {
        public InputContext(IPositionOffseter offsetter,
                            IInputHandler inputHandler)
            : base(offsetter, inputHandler)
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

                case MessageTypes.WM_LBUTTONUP:
                    _leftButtonWentDown = default;
                    _lastDragPosition = default;
                    _inputHandler.OnMouseInput(
                        new MouseUpEventArgs(GetPosition(m.LParam),
                            MouseButtons.Left, this), 
                        InputAction.LeftMouseButtonUp);
                    break;

                case MessageTypes.WM_RBUTTONUP:
                    _rightButtonWentDown = default;
                    _lastDragPosition = default;
                    _inputHandler.OnMouseInput(
                        new MouseUpEventArgs(GetPosition(m.LParam),
                        MouseButtons.Right, this), 
                        InputAction.RightMouseButtonUp);
                    break;

                case MessageTypes.WM_MOUSEWHEEL:

                    var args = new MouseWheelEventArgs(CursorPosition, 
                        (Int64)m.WParam >= Int32.MaxValue ? 1 : -1, this);

                    _inputHandler.OnMouseInput(args, InputAction.MouseWheel);

                    break;

                case MessageTypes.WM_MOUSEMOVE:

                    pos = GetPosition(m.LParam);

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

                    _lastDragPosition = pos;

                    var dragArgs = new DragEventArgs(letsUse, pos, lastDragChange,
                        _leftButtonWentDown != null ? MouseButtons.Left : MouseButtons.Right,
                        this);
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

       

        private static ValuePoint2D GetPosition(IntPtr lParam) => GetPosition((Int32) lParam);
        

        private static ValuePoint2D GetPosition(Int32 lParam)
        {
            return new ValuePoint2D((Int16) lParam,
                (Int16) (lParam >> 16)); //cast to int16 for overflow-negatives
        }
    }
}
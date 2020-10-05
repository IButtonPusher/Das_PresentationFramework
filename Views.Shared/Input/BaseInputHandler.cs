using System;
using System.Threading.Tasks;

namespace Das.Views.Input
{
    public class BaseInputHandler : IInputHandler
    {
        public BaseInputHandler(IElementLocator elementLocator)
        {
            _elementLocator = elementLocator;
        }

        public void Dispose()
        {
            //TODO_IMPLEMENT_ME();
        }

        

        public Boolean OnMouseHovering(MouseDownEventArgs args)
        {
            foreach (var clickable in _elementLocator.GetVisualsForInput<IMouseInputHandler>(
                args.Position, InputAction.MouseDown))
            {
                if (clickable.OnMouseHovering(args))
                    return true;
            }

            return false;
        }

        public Boolean OnMouseDown(MouseDownEventArgs args)
        {
            foreach (var clickable in _elementLocator.GetVisualsForMouseInput<MouseDownEventArgs>(
                args.Position, InputAction.MouseDown))
            {
                if (clickable.OnInput(args))
                    return true;
            }

            return false;
        }

        public Boolean OnMouseUp(MouseUpEventArgs args)
        {
            foreach (var clickable in _elementLocator.GetVisualsForMouseInput<MouseUpEventArgs>(
                args.Position, InputAction.MouseDown))
            {
                if (clickable.OnInput(args))
                    return true;
            }

            return false;
        }

        public void OnMouseInput<TArgs>(TArgs args, InputAction action)
            where TArgs : IMouseInputEventArgs
        {
            foreach (var clickable in _elementLocator.GetVisualsForMouseInput<TArgs>(
                args.Position, action))
            {
                //System.Diagnostics.Debug.WriteLine("routing input: " + args + " to " + clickable);

                if (clickable.OnInput(args))
                    return;
            }
        }

        public void OnKeyboardStateChanged()
        {
            //TODO_IMPLEMENT_ME();
        }

        private readonly IElementLocator _elementLocator;
    }
}
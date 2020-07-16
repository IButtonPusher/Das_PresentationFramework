using System.Linq;
//using Das.Extensions;
using Das.Gdi;
using Das.Views.Core.Geometry;
using Das.Views.Core.Input;
using Das.Views.Input;
using ViewCompiler;

namespace Das.Views.DevKit
{
    public class DevInputHandler : InputContext
    {
        public DevInputHandler(IPositionOffseter offsetter, IElementLocator elementLocator,
            DesignViewUpdater designViewUpdater) 
            : base(offsetter)
        {
            _elementLocator = elementLocator;
            _designViewUpdater = designViewUpdater;
        }

        private readonly IElementLocator _elementLocator;
        private readonly DesignViewUpdater _designViewUpdater;

        protected override void OnMouseDown(MouseButtons button, IPoint position)
        {
           UpdateSelectedItems();
        }

        protected override void OnMouseUp(MouseButtons button, IPoint position) { }

        protected override void OnKeyboardStateChanged()
        {
            if (!AreButtonsPressed(KeyboardButtons.Control,
                KeyboardButtons.Shift))
                return;

            UpdateSelectedItems();
        }

        private void UpdateSelectedItems()
        {
            var cursor = CursorPosition;
            var elements = _elementLocator.GetElementsAt(cursor).ToArray();

            var newArr = elements.ToArray();
            if (newArr.Congruent(_designViewUpdater.SelectedVisuals))
                return;

            _designViewUpdater.SelectedVisuals = newArr;
        }
    }
}

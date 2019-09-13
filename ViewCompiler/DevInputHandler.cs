using System;
using System.Linq;
using Das.CoreExtensions;
using Das.Gdi;
using Das.Views.Core.Geometry;
using Das.Views.Core.Input;
using Das.Views.Input;
using ViewCompiler;

namespace Das.Views.DevKit
{
    public class DevInputHandler : InputContext
    {
        private readonly IElementLocator _elementLocator;
        private readonly DesignViewUpdater _designViewUpdater;
        

//        public event EventHandler SelectionChanged;

        public DevInputHandler(IPositionOffseter offsetter, IElementLocator elementLocator,
            DesignViewUpdater designViewUpdater) 
            : base(offsetter)
        {
            _elementLocator = elementLocator;
            _designViewUpdater = designViewUpdater;
            //RenderKit = new NullRenderKit();
            //InputProvider =new NullInputProvider();
        }

        //public DevInputHandler(IRenderKit renderKit, IInputProvider inputProvider)
        //{
        //    //InputProvider = inputProvider;
        //    RenderKit = renderKit;
        //}

        //public IRenderKit RenderKit { get; set; }

        //public IInputProvider InputProvider { get; set; }

        protected override void OnMouseHovering(IPoint position) { }

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
            //SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
         
        }
    }
}

//using Das.Extensions;

using System;
using Das.Gdi;
using Das.Views.Core.Geometry;
using Das.Views.Input;
using ViewCompiler;

namespace Das.Views.DevKit
{
    public class DevInputProvider : InputContext
    {
        public DevInputProvider(IPositionOffseter offsetter, 
                                IElementLocator elementLocator,
                                DesignViewUpdater designViewUpdater,
                                DevInputHandler inputHandler,
                                IntPtr windowHandle) 
            : base(offsetter, inputHandler,windowHandle)
        {
            _elementLocator = elementLocator;
            _designViewUpdater = designViewUpdater;
        }

        private readonly IElementLocator _elementLocator;
        private readonly DesignViewUpdater _designViewUpdater;

        public IRenderKit RenderKit { get; set; }

        public IInputProvider InputProvider { get; set; }
       
    }
}

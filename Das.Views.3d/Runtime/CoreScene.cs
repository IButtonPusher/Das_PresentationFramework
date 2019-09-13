using System;
using System.Collections.Generic;

namespace Das.Views.Extended.Runtime
{
    public class CoreScene : IScene
    {
        public IEnumerable<IVisual3dElement> VisualElements { get; }

        public CoreScene(IEnumerable<IVisual3dElement> visualElements)
        {
            VisualElements = visualElements;
        }
    }
}

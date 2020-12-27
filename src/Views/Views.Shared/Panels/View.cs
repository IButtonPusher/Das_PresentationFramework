using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Panels
{
    public class View : ContentPanel,
                        IView
    {
        public View(IVisualBootstrapper visualBootstrapper)

            : base(visualBootstrapper)
        {
            StyleContext = visualBootstrapper.StyleContext;
        }

        public IStyleContext StyleContext { get; }
    }
}
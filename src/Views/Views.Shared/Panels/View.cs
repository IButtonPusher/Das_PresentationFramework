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
            //_dataContext = default!;
        }

        public IStyleContext StyleContext { get; }

        public override ValueSize Measure(IRenderSize availableSpace, 
                                          IMeasureContext measureContext)
        {
            return base.Measure(availableSpace, measureContext);
        }

        public override void Arrange(IRenderSize availableSpace, IRenderContext renderContext)
        {
            base.Arrange(availableSpace, renderContext);
        }


        //public virtual void SetDataContext(T dataContext)
        //{
        //    _dataContext = dataContext;
        //}

        //public new virtual T DataContext => _dataContext;

        //private T _dataContext;
    }
}
using System;
using System.Threading.Tasks;

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


        //public virtual void SetDataContext(T dataContext)
        //{
        //    _dataContext = dataContext;
        //}

        //public new virtual T DataContext => _dataContext;

        //private T _dataContext;
    }
}
using System;
using System.Threading.Tasks;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public abstract class TriggerBoundStyle : ElementStyle
    {
        protected TriggerBoundStyle(IVisualElement visual,
            IDataContext dataContext) : base(visual)
        {
            DataContext = dataContext;
        }

        protected IDataContext DataContext { get; }
    }
}
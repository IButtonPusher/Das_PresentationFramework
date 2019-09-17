using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public abstract class TriggerBoundStyle : ElementStyle
    {
        protected IDataContext DataContext { get; }

        protected TriggerBoundStyle(IVisualElement visual,
            IDataContext dataContext) : base(visual)
        {
            DataContext = dataContext;
        }
    }
}
using System;
using System.Threading.Tasks;
using Das.Views.Styles;

namespace Das.Views.Panels
{
    public class View<T> : ContentPanel<T>, IView<T>
    {
        public View(IStyleContext styleContext)
        {
            StyleContext = styleContext;
        }

        public View() : this(new BaseStyleContext(new DefaultStyle()))
        {
        }

        public IStyleContext StyleContext { get; }

        public override void Dispose()
        {
        }

        public void SetDataContext(T dataContext)
        {
            throw new NotImplementedException();
        }
    }
}
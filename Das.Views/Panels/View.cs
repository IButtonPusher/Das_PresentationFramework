using System;
using Das.Views.Styles;

namespace Das.Views.Panels
{
    public class View<T> : ContentPanel<T>, IView<T>
    {
        public IStyleContext StyleContext { get; }

        public View(IStyleContext styleContext)
        {
            StyleContext = styleContext;
        }

        public View() : this(new BaseStyleContext(new DefaultStyle()))
        {
        }
    }
}
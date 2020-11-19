using System;
using System.Threading.Tasks;
using Das.Views.Styles;

namespace Das.Views.Panels
{
    public class View<T> : ContentPanel<T>, 
                           IView<T>
    {
        public View(IVisualBootStrapper templateResolver)

        : base(templateResolver)
        {
            StyleContext = templateResolver.StyleContext;
            _dataContext = default!;
        }

        //public View(IVisualBootStrapper templateResolver) 
        //    : this(new BaseStyleContext(new DefaultStyle()), templateResolver)
        //{
        //}

        public IStyleContext StyleContext { get; }

        public override void Dispose()
        {
        }

        public virtual void SetDataContext(T dataContext)
        {
            _dataContext = dataContext;
        }

        public new virtual T DataContext => _dataContext;

        private T _dataContext;
    }
}
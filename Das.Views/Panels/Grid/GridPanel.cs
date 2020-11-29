using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Mvvm;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public class GridPanel<TItemsSource, T> : BasePanel<TItemsSource>
        where TItemsSource : INotifyingCollection<T>
    {
        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
        }

        //public override void Dispose()
        //{
        //}

        protected override Boolean OnDataContextChanging(Object? oldValue, 
                                                         Object? newValue)
        {
            return base.OnDataContextChanging(oldValue, newValue);
        }

        protected override void OnDataContextChanged(Object? newValue)
        {
            base.OnDataContextChanged(newValue);
        }

        public override ValueSize Measure(IRenderSize availableSpace,
                                          IMeasureContext measureContext)
        {
            return ValueSize.Empty;
        }

        public GridPanel(IDataBinding<TItemsSource>? binding,
                         IVisualBootstrapper visualBootstrapper)
            : base(binding, visualBootstrapper)
        {
        }

        public GridPanel(IVisualBootstrapper templateResolver)
            : base(templateResolver)
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Mvvm;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;

namespace Das.Views.Panels.Grid
{
    /// <summary>
    ///     Control where each visual in the Children collection occupies its own column.
    ///     For each record in the bound collection, a row is created
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class RepeaterGrid<TItems, TItem> : ItemsControl<TItems>
    where TItems : INotifyingCollection<TItem>
    {
        public RepeaterGrid(IVisualBootstrapper templateResolver)
            : base(templateResolver)
        {
            _controls = new Dictionary<Int32, List<IVisualElement>>();
            _sizes = new Dictionary<Int32, List<ISize>>();
            _columnWidths = new Dictionary<Int32, Double>();

        }

        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
            var rowNumbers = _controls.Keys.ToArray();
            var targetRect = new RenderRectangle(0, 0, 1, 1,
                availableSpace.Offset);

            foreach (var rowNumber in rowNumbers)
            {
                var sizeRow = _sizes[rowNumber];
                var row = _controls[rowNumber];

                var h = sizeRow.Max(r => r.Height);

                for (var c = 0; c < row.Count; c++)
                {
                    var w = _columnWidths[c];

                    var child = row[c];
                    targetRect.Width = w;
                    targetRect.Height = h;
                    renderContext.DrawElement(child, targetRect);
                    targetRect.X += w;
                }

                targetRect.Y += h;
            }
        }

        //protected override Boolean OnDataContextChanging(Object? oldValue, 
        //                                                 Object? newValue)
        //{
        //    return base.OnDataContextChanging(oldValue, newValue);
        //}

        //protected override void OnDataContextChanged(Object? newValue)
        //{
        //    base.OnDataContextChanged(newValue);
        //}

        //public override void Dispose()
        //{
        //}

        public override ValueSize Measure(IRenderSize availableSpace,
                                          IMeasureContext measureContext)
        {
            var rowNumbers = _controls.Keys.ToArray();
            var remainingSize = new RenderSize(availableSpace.Width, availableSpace.Height);

            foreach (var rowNumber in rowNumbers)
            {
                var sizeRow = _sizes[rowNumber];
                sizeRow.Clear();
                var currentRowHeight = 0.0;

                remainingSize.Width = availableSpace.Width;

                var row = _controls[rowNumber];
                for (var c = 0; c < row.Count; c++)
                {
                    var ctrlSize = row[c].Measure(remainingSize, measureContext);
                    if (ctrlSize.Width > _columnWidths[c])
                        _columnWidths[c] = ctrlSize.Width;
                    remainingSize.Width -= ctrlSize.Width;

                    currentRowHeight = Math.Max(currentRowHeight, ctrlSize.Height);
                    sizeRow.Add(ctrlSize);
                }

                remainingSize.Height -= currentRowHeight;
            }

            var widthNeeded = _columnWidths.Values.Sum(w => w);
            var heightNeeded = availableSpace.Height - remainingSize.Height;
            return new ValueSize(widthNeeded, heightNeeded);
        }

        //public override void SetBoundValue(IEnumerable<T> value)
        //{
        //    var content = Children.ToArray();
        //    //if (content == null)
        //    //    return;

        //    _columnWidths.Clear();
        //    for (var c = 0; c < content.Length; c++)
        //        _columnWidths.Add(c, 0);

        //    _controls.Clear(); //todo: more efficient
        //    var i = 0;

        //    foreach (var vm in value)
        //    {
        //        var row = new List<IVisualElement>();

        //        foreach (var c in content)
        //        {
        //            var ctrl = c.DeepCopy();
        //            if (ctrl is IBindableElement bindable)
        //                bindable.SetDataContext(vm);

        //            row.Add(ctrl);
        //        }

        //        _controls.Add(i, row);
        //        _sizes.Add(i, new List<ISize>());

        //        i++;
        //    }
        //}

        //public override void SetDataContext(Object? dataContext)
        //{
        //    DataContext = dataContext;

        //    var val = dataContext != null && Binding is { } binding
        //        ? binding.GetValue(dataContext)
        //        : Enumerable.Empty<T>();

        //    SetBoundValue(val);
        //}

        private readonly Dictionary<Int32, Double> _columnWidths;

        private readonly Dictionary<Int32, List<IVisualElement>> _controls;
        private readonly Dictionary<Int32, List<ISize>> _sizes;

        //private IDataTemplate _itemTemplate;

        //public IDataTemplate? ItemTemplate
        //{
        //    get => _itemTemplate;
        //    set => SetValue(ref _itemTemplate, 
        //        value ?? new DefaultTabItemTemplate(_visualBootStrapper));
        //}


        protected override void OnItemsChanged(Object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }

        protected override void AddNewItems(IEnumerable<TItems> items)
        {
            
        }

        protected override void OnDistributeDataContextToChildren(Object? newValue)
        {
            throw new NotImplementedException();
        }
    }
}
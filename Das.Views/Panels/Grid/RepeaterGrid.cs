using System;
using System.Collections.Generic;
using System.Linq;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels.Grid
{
    /// <summary>
    /// Control where each visual in the Children collection occupies its own column.
    /// For each record in the bound collection, a row is created
    /// </summary>
    public class RepeaterGrid<T> : BasePanel<IEnumerable<T>>
    {
        public RepeaterGrid()
        {
            _controls = new Dictionary<Int32, List<IVisualElement>>();
            _sizes = new Dictionary<Int32, List<ISize>>();
            _columnWidths = new Dictionary<Int32, Double>();
        }

        private readonly Dictionary<Int32, List<IVisualElement>> _controls;
        private readonly Dictionary<Int32, List<ISize>> _sizes;
        private readonly Dictionary<Int32, Double> _columnWidths;

        public override void SetDataContext(Object? dataContext)
        {
            DataContext = dataContext;

            var val = dataContext != null
                ? Binding.GetValue(dataContext)
                : Enumerable.Empty<T>();

            SetBoundValue(val);
        }

        public override void SetBoundValue(IEnumerable<T> value)
        {
            var content = Children;
            if (content == null)
                return;

            _columnWidths.Clear();
            for (var c = 0; c < content.Count; c++)
                _columnWidths.Add(c, 0);

            _controls.Clear(); //todo: more efficient
            var i = 0;

            foreach (var vm in value)
            {
                var row = new List<IVisualElement>();

                foreach (var c in content)
                {
                    var ctrl = c.DeepCopy();
                    if (ctrl is IBindableElement bindable)
                        bindable.SetDataContext(vm);

                    row.Add(ctrl);
                }

                _controls.Add(i, row);
                _sizes.Add(i, new List<ISize>());

                i++;
            }
        }

        public override ISize Measure(ISize availableSpace, IMeasureContext measureContext)
        {
            var rowNumbers = _controls.Keys.ToArray();
            var remainingSize = new Size(availableSpace.Width, availableSpace.Height);

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
            return new Size(widthNeeded, heightNeeded);
        }

        public override void Arrange(ISize availableSpace, IRenderContext renderContext)
        {
            var rowNumbers = _controls.Keys.ToArray();
            var targetRect = new Rectangle(0,0,1,1);

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

        public override void Dispose()
        {
            
        }
    }
}

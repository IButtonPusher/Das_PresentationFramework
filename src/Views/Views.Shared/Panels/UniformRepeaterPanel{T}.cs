using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Collections;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Mvvm;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;

namespace Das.Views.Panels
{
    /// <summary>
    ///     Panel where each child visual is displayed as a column and a row is generated
    ///     for each T in the Data Context
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class UniformRepeaterPanel : ItemsControl
    {
        public UniformRepeaterPanel(IVisualBootstrapper visualBootstrapper)
            : base(visualBootstrapper)
        {
            _rows = new List<VisualCollection>();
            _columns = new List<VisualCollection>();

            _dataContextRows = new Dictionary<Object, VisualCollection>();
            _columnRenderers = new Dictionary<IVisualElement, ColumnRenderer>();
            _columnIndexRenderers = new Dictionary<Int32, ColumnRenderer>();
            _rendererColumns = new Dictionary<ColumnRenderer, Int32>();
            _columnVisuals = new Dictionary<IVisualElement, VisualCollection>();
            _columnWidths = new Dictionary<Int32, Double>();
            _rowHeights = new Dictionary<Int32, Double>();

            _cellLock = new Object();
            _children.CollectionChanged += OnVisualsCollectionChanged;
            AddNewColumns(Children.GetAllChildren(), false);
        }

        public override void Arrange<TRenderSize>(TRenderSize availableSpace,
                                                 IRenderContext renderContext)
        {
            if (_totalWidthMeasured.IsZero())
                return;

            var widthRatio = availableSpace.Width / _totalWidthMeasured;

            lock (_cellLock)
            {
                var totalHeight = 0.0;
                for (var r = 0; r < _rowHeights.Count; r++)
                    totalHeight += _rowHeights[r];

                if (totalHeight.IsZero())
                    return;

                var x = 0.0;

                for (var col = 0; col < _columnWidths.Count; col++)
                {
                    var renderer = _columnIndexRenderers[col];

                    var width = _columnWidths[col] * widthRatio;

                    var colBounds = new ValueRenderRectangle(x, 0,
                        width,
                        availableSpace.Height,
                        new Point2D(availableSpace.Offset.X + x, availableSpace.Offset.Y));

                    renderer.Arrange(Orientations.Vertical, colBounds, renderContext);

                    x += colBounds.Width;
                }
            }
        }

        public override void Dispose()
        {
            if (DataContext is INotifyingCollection notifyingCollection)
                notifyingCollection.CollectionChanged -= OnDataContextCollectionChanged;

            base.Dispose();

            ClearVisuals();
        }

        public override ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
                                                      IMeasureContext measureContext)
        {
            lock (_cellLock)
            {
                _columnWidths.Clear();
                _rowHeights.Clear();


                _totalWidthMeasured = 0.0;

                foreach (var kvp in _columnRenderers)
                {
                    var renderer = kvp.Value;
                    var currentColumn = _rendererColumns[renderer];

                    var current = renderer.Measure(this, Orientations.Vertical,
                        availableSpace, measureContext);
                    _columnWidths[currentColumn] = current.Width;
                    _totalWidthMeasured += current.Width;

                    var rowsCount = renderer.RowHeights.Count;

                    for (var r = 0; r < rowsCount; r++)
                    {
                        var rowHeight = renderer.RowHeights[r];
                        if (!_rowHeights.TryGetValue(r, out var height) ||
                            height < rowHeight)
                            _rowHeights[r] = rowHeight;
                    }
                }

                var useHeight = 0.0;
                foreach (var h in _rowHeights.Values)
                {
                    useHeight += h;
                }

                return new ValueSize(_totalWidthMeasured, useHeight);
            }
        }


        protected override void AddNewItems(IEnumerable<Object> items)
        {
            AddNewRows(items);
        }

        protected override void ClearVisuals()
        {
            lock (_cellLock)
            {
                foreach (var row in _rows)
                {
                    row.Dispose();
                }

                _dataContextRows.Clear();

                _rows.Clear();

                foreach (var colCollection in _columns)
                {
                    colCollection.Clear(true);
                }

                _columnWidths.Clear();
                _rowHeights.Clear();
            }
        }

        protected override void OnDistributeDataContextToChildren(Object? newValue)
        {
            switch (newValue)
            {
                case null:
                    return;

                case INotifyingCollection notifyingCollection:
                    notifyingCollection.CollectionChanged += OnDataContextCollectionChanged;
                    AddNewRows(notifyingCollection.OfType<Object>());
                    break;

                case IEnumerable<Object> otherCollection:
                    AddNewRows(otherCollection);
                    break;
            }
        }

        protected override Object? OnInterceptDataContextChanging(Object? oldValue,
                                                                  Object? newValue)
        {
            if (oldValue is INotifyingCollection notifyingCollection)
                notifyingCollection.CollectionChanged -= OnDataContextCollectionChanged;

            _children.RunOnEachChild(c =>
            {
                if (!(c is IBindableElement b))
                    return;

                b.DataContext = default!;
            });

            ClearVisuals();

            return base.OnInterceptDataContextChanging(oldValue, newValue);
        }

        protected override void RemoveOldItems(IEnumerable<Object> removing)
        {
            lock (_cellLock)
            {
                foreach (var vm in removing)
                {
                    if (!_dataContextRows.TryGetValue(vm, out var row))
                        continue;

                    row.Dispose();
                    _rows.Remove(row);
                    _dataContextRows.Remove(vm);
                }
            }

            InvalidateMeasure();
        }

        private void AddNewColumn(IVisualElement newItem)
        {
            lock (_cellLock)
            {
                var columnIndex = _columns.Count;

                var newCol = new VisualCollection();
                _columns.Add(newCol);

                var renderer = new ColumnRenderer(newCol,
                    _rowHeights);
                _columnRenderers[newItem] = renderer;
                _columnIndexRenderers.Add(columnIndex, renderer);
                _rendererColumns.Add(renderer, columnIndex);
                _columnVisuals.Add(newItem, new VisualCollection());

                if (DataContext is IEnumerable<Object> valid)
                {
                    var vms = new List<Object>(valid);

                    for (var v = 0; v < vms.Count; v++)
                    {
                        var vm = vms[v];
                        BuildCell(vm, columnIndex, v);
                    }
                }
            }
        }

        /// <summary>
        ///     New column =>
        ///     1. For each existing row (vm), make a clone of the new visual
        ///     2. Add each new clone to the appropriate rows' control collection
        ///     3. Add each clone to the column's control collection
        /// </summary>
        private void AddNewColumns(IEnumerable<IVisualElement> adding)
        {
            AddNewColumns(adding, true);
        }

        private void AddNewColumns(IEnumerable<IVisualElement> adding,
                                   Boolean isInvalidate)
        {
            lock (_cellLock)
            {
                foreach (var newItem in adding)
                {
                    AddNewColumn(newItem);
                }
            }

            if (isInvalidate)
                InvalidateMeasure();
        }


        /// <summary>
        ///     New row =>
        ///     1. clone each child control,
        ///     2. add each clone to the appropriate columns' control collection
        ///     3. add each clone to the new row's control collection
        /// </summary>
        private void AddNewRows(IEnumerable<Object> vmsAdding)
        {
            lock (_cellLock)
            {
                var rowIndex = _rows.Count;

                if (_children.Count == 0)
                    switch (ItemTemplate)
                    {
                        case IMultiDataTemplate multi:

                            var addControls = multi.BuildVisuals(null);
                            _children.AddRange(addControls);

                            break;

                        case IDataTemplate dataTemplate:
                            var single = dataTemplate.BuildVisual(null);
                            if (single != null)
                                AddNewColumn(single);
                            break;
                    }

                foreach (var vm in vmsAdding)
                {
                    if (_dataContextRows.ContainsKey(vm))
                        throw new DuplicateNameException(
                            vm + " is already in collection.  Cannot have duplicates");

                    var newRow = new VisualCollection();
                    _rows.Add(newRow);


                    for (var c = 0; c < _children.Count; c++)
                        BuildCell(vm, c, rowIndex);

                    _dataContextRows.Add(vm, newRow);

                    rowIndex++;
                }
            }

            InvalidateMeasure();
        }


        private void BuildCell(Object vm,
                               Int32 columnIndex,
                               Int32 rowIndex)
        {
            switch (_children[columnIndex])
            {
                case IVisualElement template:
                    var ctrl = _visualBootstrapper.InstantiateCopy(template, vm);
                    //var ctrl = (TVisual) template.DeepCopy();
                    //ctrl.DataContext = vm;

                    _rows[rowIndex].Add(ctrl);

                    _columnVisuals[template].Add(ctrl);
                    _columns[columnIndex].Add(ctrl);

                    break;
            }
        }

        private void DisposeColumns()
        {
            lock (_cellLock)
            {
                _columns.Clear();
                _columnVisuals.Clear();
                _columnRenderers.Clear();
            }
        }

        private void OnDataContextCollectionChanged(Object sender,
                                                    NotifyCollectionChangedEventArgs e)
        {
            e.HandleCollectionChanges<Object>(RemoveOldItems, AddNewRows, ClearVisuals);
        }


        private void OnVisualsCollectionChanged(Object sender,
                                                NotifyCollectionChangedEventArgs e)
        {
            e.HandleCollectionChanges<IVisualElement>(RemoveColumns, AddNewColumns, DisposeColumns);
        }

        private void RemoveColumns(IEnumerable<IVisualElement> removing)
        {
            lock (_cellLock)
            {
                foreach (var ripVisual in removing)
                {
                    var column = _columnVisuals[ripVisual];
                    var columnIndex = _columns.IndexOf(column);

                    foreach (var row in _rows)
                    {
                        row.RemoveAt(columnIndex);
                    }

                    _columns.RemoveAt(columnIndex);
                    _columnVisuals.Remove(ripVisual);
                }
            }

            InvalidateMeasure();
        }

        private readonly Object _cellLock;
        private readonly Dictionary<Int32, ColumnRenderer> _columnIndexRenderers;
        private readonly Dictionary<IVisualElement, ColumnRenderer> _columnRenderers;
        private readonly List<VisualCollection> _columns;

        private readonly Dictionary<IVisualElement, VisualCollection> _columnVisuals;


        private readonly Dictionary<Int32, Double> _columnWidths;
        private readonly Dictionary<Object, VisualCollection> _dataContextRows;
        private readonly Dictionary<ColumnRenderer, Int32> _rendererColumns;
        private readonly Dictionary<Int32, Double> _rowHeights;

        private readonly List<VisualCollection> _rows;

        private Double _totalWidthMeasured;
    }
}

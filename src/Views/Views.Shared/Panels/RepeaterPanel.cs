using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Collections;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.ItemsControls;
using Das.Views.Mvvm;
using Das.Views.Rendering;
// ReSharper disable UnusedMember.Global

namespace Das.Views.Panels
{
    // ReSharper disable once UnusedType.Global
    public class RepeaterPanel : ContentPanel,
                                 IRepeaterPanel,
                                 IItemsControl
    {
        public RepeaterPanel(IVisualBootstrapper visualBootstrapper)
            : this(visualBootstrapper, null, null)
        {
        }

        // ReSharper disable once UnusedMember.Global
        public RepeaterPanel(IVisualBootstrapper visualBootstrapper,
                             ISequentialRenderer? renderer,
                             IVisualCollection? children)
            : base(visualBootstrapper)
        {
            switch (children)
            {
                case VisualCollection good:
                    _controls = good;
                    break;

                case { } someCollection:
                    _controls = new VisualCollection(someCollection);
                    break;

                default:
                    _controls = new VisualCollection();
                    break;
            }

            _controlsLock = new Object();
            _renderer = EnsureRenderer(renderer);
            _visualsByVm = new Dictionary<Object, IVisualElement>();
            _itemsControlHelper = new ItemsControlHelper(AddNewItems, RemoveItems, ClearItems,
                null, null);

            var wot = Interlocked.Add(ref _netAlive, 1);

            if (wot > 1000)
            {}
        }

        private static Int32 _netAlive;

        INotifyingCollection? IItemsControl.ItemsSource => ItemsSource;

        IDataTemplate? IItemsControl.ItemTemplate => ItemTemplate;

        public override ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
                                                      IMeasureContext measureContext)
        {
            var res = _renderer.Measure(this, Orientation, availableSpace, measureContext);
            return res;
        }

        public override void Arrange<TRenderSize>(TRenderSize availableSpace,
                                                 IRenderContext renderContext)
        {
            _renderer.Arrange(Orientation, availableSpace.ToFullRectangle(), renderContext);
        }

        public override void Dispose()
        {
            base.Dispose();

            _controls.Dispose();

            lock (_controlsLock)
               _visualsByVm.Clear();

            Interlocked.Add(ref _netAlive, -1);
        }

        public Orientations Orientation { get; set; }

        public override IVisualElement? Content { get; set; }

        IVisualCollection ISequentialPanel.Children => _controls;

        public INotifyingCollection? ItemsSource
        {
            get => _itemsControlHelper.ItemsSource;
            set => _itemsControlHelper.ItemsSource = value;
        }

        public IDataTemplate? ItemTemplate
        {
            get => _itemsControlHelper.ItemTemplate;
            set => _itemsControlHelper.ItemTemplate = value;
        }

        protected override void OnDataContextChanged(Object? newValue)
        {
            RefreshBoundValues(newValue);
            InvalidateMeasure();
        }

        protected override Object? OnInterceptDataContextChanging(Object? oldValue,
                                                                  Object? newValue)
        {
            ClearBeforeSet();
            return base.OnInterceptDataContextChanging(oldValue, newValue);
        }

        private void AddNewItems(IEnumerable<Object> value)
        {
            if (!(Content is { } content))
                return;


            lock (_controlsLock)
            {
                foreach (var vm in value)
                {
                    var ctrl = _visualBootstrapper.InstantiateCopy(content, vm);
                    _visualsByVm.Add(vm, ctrl);

                    _controls.Add(ctrl);
                }
            }
        }


        private IVisualElement? ClearBeforeSet()
        {
            var content = Content;
            if (content == null)
                return default;

            _controls.Clear(true);

            return content;
        }

        private void ClearItems()
        {
            lock (_controlsLock)
            {
                _visualsByVm.Clear();
            }

            _controls.Clear(true);
        }

        private ISequentialRenderer EnsureRenderer(ISequentialRenderer? input)
        {
            return input ?? new SequentialRenderer(_controls);
        }

        private void RemoveItems(IEnumerable<Object> value)
        {
            lock (_controlsLock)
            {
                foreach (var vm in value)
                {
                    if (_visualsByVm.TryGetValue(vm, out var ctrl))
                    {
                        _visualsByVm.Remove(vm);
                        _controls.Remove(ctrl);
                    }

                    _controls.Add(ctrl);
                }
            }
        }

        private readonly VisualCollection _controls;
        private readonly Object _controlsLock;

        private readonly ItemsControlHelper _itemsControlHelper;

        private readonly ISequentialRenderer _renderer;
        private readonly Dictionary<Object, IVisualElement> _visualsByVm;
    }
}
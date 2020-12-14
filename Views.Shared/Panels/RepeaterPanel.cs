using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Collections;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.ItemsControls;
using Das.Views.Mvvm;
using Das.Views.Rendering;
using Das.Views.Templates;

namespace Das.Views.Panels
{
    public class RepeaterPanel : ContentPanel, 
                                    IRepeaterPanel,
                                    IItemsControl
    {
        public RepeaterPanel(IVisualBootstrapper visualBootstrapper)
            : this(visualBootstrapper, null, null)
        {

        }
        
        //public RepeaterPanel(IDataBinding<IEnumerable<T>>? binding,
        //                     IVisualBootstrapper visualBootstrapper)
        //: this(binding, visualBootstrapper, null, null)
        //{

        //}
        
        
        // ReSharper disable once UnusedMember.Global
        public RepeaterPanel(//IDataBinding<IEnumerable<T>>? binding,
                             IVisualBootstrapper visualBootstrapper,
                             ISequentialRenderer? renderer,
                             IVisualCollection? children) 
            : base(visualBootstrapper)//, binding)
        {
            switch (children)
            {
                case VisualCollection good:
                    _controls = good;
                    break;
                
                case {} someCollection:
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
            
        }
        
        //public RepeaterPanel(IVisualBootstrapper visualBootstrapper,
        //                     ISequentialRenderer renderer,
        //                     IVisualCollection children) 
        
        //: this(null, visualBootstrapper, renderer, children)
        //{}
        
        //private RepeaterPanel(IVisualBootstrapper visualBootstrapper,
        //                      IVisualCollection children)
        //    : this(new SequentialRenderer(children), visualBootstrapper)

        //{
        //    _controls = children is VisualCollection good 
        //        ? good 
        //        : new VisualCollection(children);
        //}
        
        //// ReSharper disable once UnusedMember.Global
        //public RepeaterPanel(IVisualBootstrapper visualBootstrapper) 
        //    : this(visualBootstrapper, new VisualCollection())
        //{
        //}

      

        //public RepeaterPanel(ISequentialRenderer renderer,
        //                     IVisualBootstrapper visualBootstrapper)
        //: base(visualBootstrapper)
        //{
            
            
        //    _controls = new VisualCollection();
        //    _renderer = EnsureRenderer(renderer);
        //}

        //public RepeaterPanel(IDataBinding<T> binding,
        //                     IVisualBootstrapper visualBootstrapper) 
        //    // ReSharper disable once IntroduceOptionalParameters.Global
        //    : this(binding, visualBootstrapper, null)
        //{
            
        //}

        

        //public override void SetBoundValue(Object? value)
        //{
        //    if (value is IEnumerable<T> goodOne)
        //        SetBoundValue(goodOne);
        //    else throw new NotImplementedException();
        //}

        protected override Object? OnInterceptDataContextChanging(Object? oldValue, 
                                                                  Object? newValue)
        {
            ClearBeforeSet();
            return base.OnInterceptDataContextChanging(oldValue, newValue);
        }

        protected override void OnDataContextChanged(Object? newValue)
        {


            RefreshBoundValues(newValue);
            InvalidateMeasure();

            //if (!(ClearBeforeSet() is {} content))
            //    return;
        }

        //public override void SetBoundValue(Object? oValue)
        //{
        //    if (!(ClearBeforeSet() is {} content))
        //        return;

        //    if (oValue is IEnumerable<T> value)
        //    {
        //        foreach (var vm in value)
        //        {
        //            var ctrl = content.DeepCopy();
        //            if (ctrl is IBindableElement bindable) bindable.SetBoundValue(vm!);

        //            _controls.Add(ctrl);
        //        }
        //    }
        //}

        //public override async Task SetBoundValueAsync(Object? oValue)
        //{
        //    if (!(ClearBeforeSet() is {} content))
        //        return;

        //    if (oValue is IEnumerable<T> value)
        //    {
        //        foreach (var vm in value)
        //        {
        //            var ctrl = content.DeepCopy();
        //            if (ctrl is IBindableElement bindable)
        //                await bindable.SetBoundValueAsync(vm!);

        //            _controls.Add(ctrl);
        //        }
        //    }
        //}

        private void ClearItems()
        {
            lock (_controlsLock)
                _visualsByVm.Clear();

            _controls.Clear(true);

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
                    
                    //var ctrl = _visualBootstrapper.InstantiateCopy(content, vm);
                    //var ctrl = content.DeepCopy();
                    //if (ctrl is IBindableElement bindable)
                    //    bindable.DataContext = vm;

                    _controls.Add(ctrl);
                }
            }
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
                    //var ctrl = content.DeepCopy();
                    //if (ctrl is IBindableElement bindable)
                    //    bindable.DataContext = vm;

                    _controls.Add(ctrl);
                }
            }
        }
        
        public override ValueSize Measure(IRenderSize availableSpace,
                                          IMeasureContext measureContext)
        {
            var res = _renderer.Measure(this, Orientation, availableSpace, measureContext);
            return res;
        }

        public override void Arrange(IRenderSize availableSpace, 
                                     IRenderContext renderContext)
        {
            _renderer.Arrange(Orientation, availableSpace.ToFullRectangle(), renderContext);
        }

        public override void Dispose()
        {
            base.Dispose();

            _controls.Dispose();

            //for (var c = 0; c < _controls.Count; c++)
            //    _controls[c].Dispose();

            //_controls.Clear();
        }

        //public IList<IVisualElement> Children => _controls;

       

        //public override void SetDataContext(Object? dataContext)
        //{
        //    DataContext = dataContext;
        //    if (!(Binding is {} binding))
        //        return;

        //    if (!(dataContext is {} dc))
        //        SetBoundValue(Enumerable.Empty<T>());
        //    else
        //        SetBoundValue(binding.GetValue(dc));
        //}

        //public override async Task SetDataContextAsync(Object? dataContext)
        //{
        //    //DataContext = dataContext;
        //    if (!(Binding is {} binding))
        //        return;

        //    if (!(dataContext is {} dc))
        //        await SetBoundValueAsync(Enumerable.Empty<T>());
        //    else
        //        await SetBoundValueAsync(await binding.GetValueAsync(dc));
        //}

        public Orientations Orientation { get; set; }

        public override IVisualElement? Content { get; set; }

        //IBindableElement<IEnumerable<T>>? IRepeaterPanel<T>.Content =>
        //    Content as IBindableElement<IEnumerable<T>>;

        
        
        private IVisualElement? ClearBeforeSet()
        {
            var content = Content;
            if (content == null)
                return default;

            _controls.Clear(true);

            //for (var c = 0; c < _controls.Count; c++)
            //    _controls[c].Dispose();

            //_controls.Clear(); //todo: more efficient

            return content;
        }

        private ISequentialRenderer EnsureRenderer(ISequentialRenderer? input)
        {
            return input ?? new SequentialRenderer(_controls);
        }

        //public override void SetBoundValue(IEnumerable<T> value)
        //{
        //    if (!(ClearBeforeSet() is { } content))
        //        return;

        //    foreach (var vm in value)
        //    {
        //        var ctrl = content.DeepCopy();
        //        if (ctrl is IBindableElement bindable) bindable.SetBoundValue(vm!);

        //        _controls.Add(ctrl);
        //    }
        //}

        //public override async Task SetBoundValueAsync(IEnumerable<T> value)
        //{
        //    if (!(ClearBeforeSet() is { } content))
        //        return;

        //    foreach (var vm in value)
        //    {
        //        var ctrl = content.DeepCopy();
        //        if (ctrl is IBindableElement bindable)
        //            await bindable.SetBoundValueAsync(vm!);

        //        _controls.Add(ctrl);
        //    }
        //}

        //private readonly List<IVisualElement> _controls;
        private readonly ISequentialRenderer _renderer;
        private readonly VisualCollection _controls;
        private readonly Dictionary<Object, IVisualElement> _visualsByVm;
        private readonly Object _controlsLock;

        IVisualCollection ISequentialPanel.Children => _controls;
        //private INotifyingCollection<T>? _itemsSource;

        //INotifyingCollection? IItemsControl.ItemsSource => ItemsSource;

        //public INotifyingCollection<T>? ItemsSource
        //{
        //    get => _itemsSource;
        //    set => SetValue(ref _itemsSource, value,
        //        OnItemsSourceChanging);
        //}

        //public IDataTemplate? ItemTemplate
        //{
        //    get => _itemTemplate;
        //    set => SetValue(ref _itemTemplate,
        //        value, OnItemTemplateChanged);
        //}
        
        //protected virtual void OnItemTemplateChanged(IDataTemplate? obj)
        //{
            
        //}
        
        
        private readonly ItemsControlHelper _itemsControlHelper;

        INotifyingCollection? IItemsControl.ItemsSource => ItemsSource;

        IDataTemplate? IItemsControl.ItemTemplate => ItemTemplate;

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
    }
}
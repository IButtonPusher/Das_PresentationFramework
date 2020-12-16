using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Rendering;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;

#endif

namespace Das.Views.DataBinding
{
    public abstract class BindableElement : VisualElement,
                                            IBindableElement
    {
        protected BindableElement(IVisualBootstrapper visualBootstrapper) 
            : base(visualBootstrapper)
           // : this(null, visualBootstrapper)
        {
            //BindingHelper = new BindingHelper(this, 
            //    OnInterceptDataContextChanging, OnDataContextChanged);

            //_binding = binding;
            _lockBindings = new Object();
            _openBindings = new List<IDataBinding>();
            _bindings = new List<IDataBinding>();
        }

        //protected BindableElement(IVisualBootstrapper visualBootstrapper,
        //                          IBindable bindingHelper)
        //: base(visualBootstrapper)
        //{
        //    _bindingHelper = bindingHelper;
        //}

        //protected BindableElement(IDataBinding? binding,
        //                          IVisualBootstrapper visualBootstrapper) 
        //    : base(visualBootstrapper)
        //{
        //    _binding = binding;
        //    _lockBindings = new Object();
        //    _openBindings = new List<IDataBinding>();
        //    _bindings = new List<IDataBinding>();
        //}

        public Boolean TryGetDataContextBinding(out IDataBinding dcBinding)
        {
            lock (_lockBindings)
            {
                foreach (var binding in _bindings)
                {
                    if (binding.IsDataContextBinding)
                    {
                        dcBinding = binding;
                        return true;
                    }
                }
            }

            dcBinding = default!;
            return false;
        }
        
        public Object? Value => DataContext;

        //        public virtual void SetBoundValue(Object? value)
        //        {
        //            var binding = _binding;
        //            throw new NotImplementedException();
        //            //if (binding == null || binding is InstanceBinding)
        //            //    _binding = new ObjectBinding(value);

        ////            BoundValue = value;
        //        }

        //        public virtual Task SetBoundValueAsync(Object? value)
        //        {
        //            SetBoundValue(value);
        //            return TaskEx.CompletedTask;
        //        }

        //        public virtual void SetDataContext(Object? dataContext)
        //        {
        //            var binding = _binding;
        //            if (binding == null)
        //                return;

        //            DataContext = dataContext;
        //            if (dataContext == null)
        //                return;

        //            var val = binding.GetBoundValue(dataContext);
        //            SetBoundValue(val);
        //        }

        //        public virtual async Task SetDataContextAsync(Object? dataContext)
        //        {
        //            var binding = _binding;
        //            if (binding == null)
        //                return;

        //            DataContext = dataContext;
        //            if (dataContext == null)
        //                return;

        //            var val = binding.GetBoundValue(dataContext);
        //            await SetBoundValueAsync(val);
        //        }

        public void AddBinding(IDataBinding binding)
        {
            lock (_lockBindings)
            {
                _openBindings.Add(binding);
                _bindings.Add(binding);
            }
        }

        public IEnumerable<IDataBinding> GetBindings()
        {
            List<IDataBinding> res;

            lock (_lockBindings)
            {
                res = new List<IDataBinding>();
                foreach (var b in _bindings)
                {
                    var bClone = (IDataBinding)b.Clone();
                    res.Add(bClone);
                }
            }

            return res;
        }

        public override void Dispose()
        {
            base.Dispose();

            //BindingHelper.Dispose();

            foreach (var binding in _bindings)
                binding.Dispose();

            _bindings.Clear();

           // Binding = default;
        }

        //public IDataBinding? Binding { get; set; }

        //public Object? DataContext { get; set; }

        private Object? _dataContext;

        public virtual Object? DataContext
        {
            get => _dataContext;
            set => SetValue(ref _dataContext, value,
                OnInterceptDataContextChanging, OnDataContextChanged);
        }

        protected virtual void OnDataContextChanged(Object? newValue)
        {
            RefreshBoundValues(newValue);
            InvalidateMeasure();
        }

        protected virtual void RefreshBoundValues(Object? dataContext)
        {
            lock (_lockBindings)
            {
                for (var c = 0; c < _bindings.Count; c++)
                {
                    var b = _openBindings[c];

                    if (b is IVisualPropertySetter { PropertyName: nameof(DataContext) } propSetter)
                        continue;

                    _bindings[c] = b.Update(dataContext, this);
                }
            }
        }

        protected virtual  Object? OnInterceptDataContextChanging(Object? oldValue, 
                                                                  Object? newValue)
        {
            if (newValue is { } something)
            {
                lock (_lockBindings)
                {
                    foreach (var binding in _bindings)
                    {
                        if (binding is IVisualPropertySetter
                            { PropertyName: nameof(DataContext) } setter)
                        {
                            var res = setter.GetSourceValue(something);
                            //  if (res != null)
                            return res;
                        }
                    }
                }
            }

            return newValue;
        }

        //public virtual Object? GetBoundValue(Object dataContext)
        //{
        //    switch (BoundValue)
        //    {
        //        case { } set:
        //            return set;
        //        default:
        //            if (_binding != null)
        //                BoundValue = _binding.GetBoundValue(dataContext);
        //            return BoundValue;
        //    }
        //}


        //private IDataBinding? _binding;
        protected readonly List<IDataBinding> _bindings;
        private readonly List<IDataBinding> _openBindings;
        protected readonly Object _lockBindings;

        //protected abstract IBindable BindingHelper {get;}

        //protected readonly BindingHelper _bindingHelper;
        //protected Object? BoundValue;
        //public void AddBinding(IDataBinding binding)
        //{
        //    BindingHelper.AddBinding(binding);
        //}

        //public IEnumerable<IDataBinding> GetBindings()
        //{
        //    return BindingHelper.GetBindings();
        //}

        //public Object? DataContext
        //{
        //    get => BindingHelper.DataContext;
        //    set => BindingHelper.DataContext = value;
        //}

        //public event Func<Object?, Object?, Object?> InterceptDataAContextChange
        //{
        //    add => _bindingHelper.InterceptDataAContextChange += value;
        //    remove => _bindingHelper.InterceptDataAContextChange -= value;
        //}

        //public event Action<Object?> DataContextChanged
        //{
        //    add => _bindingHelper.DataContextChanged += value;
        //    remove => _bindingHelper.DataContextChanged -= value;
        //}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Rendering;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#pragma warning disable 8618
#endif

namespace Das.Views.DataBinding
{
    public abstract class BindableElement<T> : VisualElement, 
                                               IBindableElement<T>
    {
        protected BindableElement(IDataBinding<T>? binding,
                                  IVisualBootstrapper visualBootstrapper)
        : base(visualBootstrapper)
        {
            Binding = binding;
            _binding = binding;
            _visualBootstrapper = visualBootstrapper;
            _bindings = new List<IDataBinding>();
            _lockBindings = new Object();
        }

        protected BindableElement(IVisualBootstrapper visualBootstrapper) 
            : this(null, visualBootstrapper)
        {
            
        }

        public virtual Task SetBoundValueAsync(T value)
        {
            SetBoundValue(value);
            return TaskEx.CompletedTask;
        }

        public void AddBinding(IDataBinding binding)
        {
            lock (_lockBindings)
                _bindings.Add(binding);

            binding.Evaluate();
        }

        public IDataBinding<T>? Binding
        {
            get => _binding;
            set => SetValue(ref _binding, value, OnBindingChanging, OnBindingChanged);
        }

        protected virtual void OnBindingChanged(IDataBinding<T>? obj)
        {
            InvalidateMeasure();
        }

        protected virtual Boolean OnBindingChanging(IDataBinding<T>? oldValue, 
                                                    IDataBinding<T>? newValue)
        {
            if (newValue == null && oldValue != null)
            {}

            if (oldValue is {} old)
                old.Dispose();

            return true;
        }

        IDataBinding? IBindableElement.Binding
        {
            get => Binding;
            set => Binding = value as IDataBinding<T> ?? Binding;
        }


        private Object? _dataContext;

        public virtual Object? DataContext
        {
            get => _dataContext;
            set => SetValue(ref _dataContext, value,
                OnDataContextChanging, OnDataContextChanged);
        }

        protected virtual void OnDataContextChanged(Object? newValue)
        {
            RefreshBoundValues(newValue);
            InvalidateMeasure();
        }

        protected virtual  Boolean OnDataContextChanging(Object? oldValue, 
                                                         Object? newValue)
        {
            return true;
        }

        //public Object? DataContext { get; set; }

        T IBindableElement<T>.GetBoundValue(Object dataContext)
        {
            return GetBoundValue(dataContext);
        }

        public virtual void SetBoundValue(T value)
        {
            var binding = _binding;
            if (binding == null || binding is InstanceBinding<T>) _binding = new ObjectBinding<T>(value);

            BoundValue = value;
        }

        public virtual void SetBoundValue(Object? value)
        {
            if (value is T works)
                SetBoundValue(works);
            else if (value != null)
                throw new NotSupportedException();
        }

        public virtual async Task SetBoundValueAsync(Object? value)
        {
            if (value is T works)
                await SetBoundValueAsync(works);
            else if (value != null)
                throw new NotSupportedException();
        }

        public virtual void SetDataContext(Object? dataContext)
        {
            //var binding = _binding;
            //if (binding == null)
            //    return;

            DataContext = dataContext;
            //if (dataContext == null)
            //    return;

            //var val = binding.GetValue(dataContext);
            //SetBoundValue(val);
        }

        public virtual Task SetDataContextAsync(Object? dataContext)
        {
            DataContext = dataContext;
            return TaskEx.CompletedTask;
        }

        public override IVisualElement DeepCopy()
        {
            var res0 = base.DeepCopy();
            if (res0 is IBindableElement bindable)
                bindable.Binding = Binding;
            return res0;
        }

        Object? IBindableElement.GetBoundValue(Object dataContext)
        {
            if (_binding is {} b)
                return b.GetValue(dataContext);

            return null;
        }

        public Object? Value => DataContext;


        protected T GetBoundValue(Object dataContext)
        {
            switch (BoundValue)
            {
                case T set:
                    return set;
                default:
                    if (_binding != null)
                        BoundValue = _binding.GetValue(dataContext);
                    return BoundValue;
            }
        }

        protected virtual void RefreshBoundValues(Object? dataContext)
        {
            if (_binding is {} binding)
            {
                var val = binding.GetValue(dataContext);
                SetBoundValue(val);
            }

            lock (_lockBindings)
            {
                //foreach (var b in _bindings)
                for (var c = 0; c < _bindings.Count; c++)
                {
                    var b = _bindings[c];
                    _bindings[c] = b.Update(dataContext, this);
                }
            }
        }

        private void SetBinding(IDataBinding<T>? value)
        {
            _binding = value;
        }

        public Boolean Equals(IBindableElement<T> other)
        {
            return ReferenceEquals(this, other);
        }

        public override String ToString()
        {
            var str = GetType().Name;
            var gargs = GetType().GetGenericArguments().FirstOrDefault();
            if (gargs != null)
                str += "[" + gargs.Name + "]";

            return str;
        }

        public override void Dispose()
        {
            base.Dispose();

            lock (_lockBindings)
            {
                foreach (var binding in _bindings)
                    binding.Dispose();

                _bindings.Clear();
            }

            Binding = default;
        }


        private IDataBinding<T>? _binding;
        private readonly IVisualBootstrapper _visualBootstrapper;
        private readonly List<IDataBinding> _bindings;
        private readonly Object _lockBindings;
        protected T BoundValue;
    }
}

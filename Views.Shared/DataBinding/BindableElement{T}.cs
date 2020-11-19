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
                                  IVisualBootStrapper templateResolver)
        : base(templateResolver)
        {
            _binding = binding;
            _bindings = new List<IDataBinding>();
        }

        protected BindableElement(IVisualBootStrapper templateResolver) 
            : base(templateResolver)
        {
            _bindings = new List<IDataBinding>();
        }

        public virtual Task SetBoundValueAsync(T value)
        {
            SetBoundValue(value);
            return TaskEx.CompletedTask;
        }

        public void AddBinding(IDataBinding binding)
        {
            _bindings.Add(binding);
        }

        public IDataBinding<T>? Binding
        {
            get => _binding;
            set => SetBinding(value);
        }

        IDataBinding? IBindableElement.Binding
        {
            get => Binding;
            set => Binding = value as IDataBinding<T> ?? Binding;
        }

        public Object? DataContext { get; set; }

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
            var binding = _binding;
            if (binding == null)
                return;

            DataContext = dataContext;
            if (dataContext == null)
                return;

            var val = binding.GetValue(dataContext);
            SetBoundValue(val);
        }

        public virtual async Task SetDataContextAsync(Object? dataContext)
        {
            var binding = _binding;
            if (binding == null)
                return;

            DataContext = dataContext;
            if (dataContext == null)
                return;

            var val = await binding.GetValueAsync(dataContext);
            await SetBoundValueAsync(val);
        }

        public override IVisualElement DeepCopy()
        {
            var newObject = base.DeepCopy();

            if (newObject is BindableElement<T> alsoBindable && Binding != null)
            {
                var newBinding = Binding.DeepCopy();
                alsoBindable.SetBinding(newBinding);
            }

            return newObject;
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

        private void SetBinding(IDataBinding<T>? value)
        {
            _binding = value;
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
            foreach (var binding in _bindings)
                binding.Dispose();
            
            _bindings.Clear();
        }


        private IDataBinding<T>? _binding;
        private List<IDataBinding> _bindings;
        protected T BoundValue;
    }
}

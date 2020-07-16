using System;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Rendering;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace Das.Views.DataBinding
{
    public abstract class BindableElement<T> : VisualElement, IBindableElement<T>
    {
        protected BindableElement(IDataBinding<T> binding)
        {
            _binding = binding;
        }

        protected BindableElement()
        {
        }

        public override String ToString()
        {
            var str = GetType().Name;
            var gargs = GetType().GetGenericArguments().FirstOrDefault();
            if (gargs != null)
                str += "[" + gargs.Name + "]";

            return str;
        }


        protected T GetBoundValue(Object dataContext)
        {
            switch (BoundValue)
            {
                case T set:
                    return set;
                default:
                    BoundValue = _binding.GetValue(dataContext);
                    return BoundValue;
            }
        }

        private void SetBinding(IDataBinding<T> value)
        {
            _binding = value;
        }

        public virtual Task SetBoundValueAsync(T value)
        {
            SetBoundValue(value);
            return TaskEx.CompletedTask;
        }

        public IDataBinding<T> Binding
        {
            get => _binding;
            set => SetBinding(value);
        }

        IDataBinding IBindableElement.Binding
        {
            get => Binding;
            set => Binding = value as IDataBinding<T> ?? Binding;
        }

        public Object DataContext { get; set; }

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

        public virtual void SetBoundValue(Object value)
        {
            if (value is T works)
                SetBoundValue(works);
            else if (value != null)
                throw new NotSupportedException();
        }

        public virtual async Task SetBoundValueAsync(Object value)
        {
            if (value is T works)
                await SetBoundValueAsync(works);
            else if (value != null)
                throw new NotSupportedException();
        }

        public virtual void SetDataContext(Object dataContext)
        {
            var binding = _binding;
            if (binding == null)
                return;

            DataContext = dataContext;
            var val = _binding.GetValue(dataContext);
            SetBoundValue(val);
        }

        public virtual async Task SetDataContextAsync(Object dataContext)
        {
            var binding = _binding;
            if (binding == null)
                return;

            DataContext = dataContext;
            var val = await _binding.GetValueAsync(dataContext);
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

        Object IBindableElement.GetBoundValue(Object dataContext)
        {
            return _binding.GetValue(dataContext);
        }

        public Object Value => DataContext;


        private IDataBinding<T> _binding;
        protected T BoundValue;
    }
}
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
        protected BindableElement(IVisualBootStrapper visualBootStrapper) 
            : this(null, visualBootStrapper)
        {
            _bindings = new List<IDataBinding>();
        }

        protected BindableElement(IDataBinding? binding,
                                  IVisualBootStrapper visualBootStrapper) 
            : base(visualBootStrapper)
        {
            _binding = binding;
            _bindings = new List<IDataBinding>();
        }

        public Object? Value => DataContext;

        public virtual void SetBoundValue(Object? value)
        {
            var binding = _binding;
            throw new NotImplementedException();
            //if (binding == null || binding is InstanceBinding)
            //    _binding = new ObjectBinding(value);

            BoundValue = value;
        }

        public virtual Task SetBoundValueAsync(Object? value)
        {
            SetBoundValue(value);
            return TaskEx.CompletedTask;
        }

        public virtual void SetDataContext(Object? dataContext)
        {
            var binding = _binding;
            if (binding == null)
                return;

            DataContext = dataContext;
            if (dataContext == null)
                return;

            var val = binding.GetBoundValue(dataContext);
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

            var val = binding.GetBoundValue(dataContext);
            await SetBoundValueAsync(val);
        }

        public void AddBinding(IDataBinding binding)
        {
            _bindings.Add(binding);
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (var binding in _bindings)
                binding.Dispose();

            _bindings.Clear();
            
        }

        public IDataBinding? Binding { get; set; }

        public Object? DataContext { get; set; }

        public virtual Object? GetBoundValue(Object dataContext)
        {
            switch (BoundValue)
            {
                case { } set:
                    return set;
                default:
                    if (_binding != null)
                        BoundValue = _binding.GetBoundValue(dataContext);
                    return BoundValue;
            }
        }

        
        private IDataBinding? _binding;
        private List<IDataBinding> _bindings;
        protected Object? BoundValue;
    }
}
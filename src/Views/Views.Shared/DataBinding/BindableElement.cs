using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Controls;
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
        {
            _lockBindings = new Object();
            _openBindings = new List<IDataBinding>();
            _bindings = new List<IDataBinding>();
        }

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
                    var bClone = (IDataBinding) b.Clone();
                    res.Add(bClone);
                }
            }

            return res;
        }

        public override void Dispose()
        {
            base.Dispose();


            foreach (var binding in _bindings)
                binding.Dispose();

            _bindings.Clear();
        }

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

        protected virtual Object? OnInterceptDataContextChanging(Object? oldValue,
                                                                 Object? newValue)
        {
            if (newValue is { } something)
                lock (_lockBindings)
                {
                    foreach (var binding in _bindings)
                        if (binding is IVisualPropertySetter
                            {PropertyName: nameof(DataContext)} setter)
                        {
                            var res = setter.GetSourceValue(something);
                            return res;
                        }
                }

            return newValue;
        }

        protected virtual void RefreshBoundValues(Object? dataContext)
        {
            lock (_lockBindings)
            {
                for (var c = 0; c < _bindings.Count; c++)
                {
                    var b = _openBindings[c];

                    if (b is IVisualPropertySetter {PropertyName: nameof(DataContext)})
                        continue;

                    _bindings[c] = b.Update(dataContext, this);
                }
            }
        }

        protected override void OnTemplateSet(IVisualTemplate? newValue)
        {
            base.OnTemplateSet(newValue);

            if (!(newValue?.Content is IBindableElement bindable))
                return;

            bindable.DataContext = this;
        }

        protected readonly List<IDataBinding> _bindings;
        protected readonly Object _lockBindings;
        private readonly List<IDataBinding> _openBindings;


        private Object? _dataContext;
    }
}
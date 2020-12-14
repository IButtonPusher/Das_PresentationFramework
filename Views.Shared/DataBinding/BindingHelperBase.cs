using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.Mvvm;
using Das.Views.Rendering;

namespace Das.Views.DataBinding
{
    public abstract class BindingHelperBase : NotifyPropertyChangedBase
    {
        protected readonly IVisualElement _visual;

        public BindingHelperBase(IVisualElement visual)
        {
            _lockBindings = new Object();
            _bindings = new List<IDataBinding>();
            _openBindings = new List<IDataBinding>();
            _visual = visual;
        }
        
        public void AddBinding(IDataBinding binding)
        {
            lock (_lockBindings)
                _bindings.Add(binding);
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
        
        protected virtual void RefreshBoundValues(Object? dataContext)
        {
            //if (_binding is {} binding)
            //{
            //    var val = binding.GetValue(dataContext);
            //    SetBoundValue(val);
            //}

            lock (_lockBindings)
            {
                for (var c = 0; c < _bindings.Count; c++)
                {
                    var b = _openBindings[c];
                    
                    if (b is IVisualPropertySetter {PropertyName: nameof(IBindable<Object>.DataContext)} propSetter)
                        continue;
                    
                    _bindings[c] = b.Update(dataContext, _visual);
                }
            }
        }
        
        
        protected readonly List<IDataBinding> _bindings;
        private readonly List<IDataBinding> _openBindings;
        protected readonly Object _lockBindings;
    }
}

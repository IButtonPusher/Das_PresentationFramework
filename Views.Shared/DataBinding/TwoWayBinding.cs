using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace Das.Views.DataBinding
{
    public class TwoWayBinding : SourceBinding
    {
        public TwoWayBinding(INotifyPropertyChanged source,
                             String sourceProperty,
                             IBindableElement target,
                             String targetProperty)
            : this(source,
                GetObjectPropertyOrDie(source, sourceProperty),
                target,
                GetObjectPropertyOrDie(target, targetProperty)) { }

        public TwoWayBinding(INotifyPropertyChanged source,
                             PropertyInfo srcProp,
                             IBindableElement target,
                             PropertyInfo targetProp)
        : base(source, srcProp, target, targetProp)
        {
            _sourceSetter = srcProp.GetSetMethod()
                            ?? throw new MissingMethodException(srcProp.Name);
           
            target.PropertyChanged += OnTargetPropertyChanged;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _target.PropertyChanged -= OnTargetPropertyChanged;
        }

        private void OnTargetPropertyChanged(Object sender,
                                             PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _targetPropertyName) 
                return;

            //var val = _targetGetter.Invoke(_target, EmptyObjectArray);
            var val = GetTargetValue();
            SetSourceValue(val);
            //_sourceSetter.Invoke(_source, new[] {val});
        }

        protected void SetSourceValue(Object? value) => _sourceSetter.Invoke(_source, new[] {value});

        private readonly MethodInfo _sourceSetter;
    }
}
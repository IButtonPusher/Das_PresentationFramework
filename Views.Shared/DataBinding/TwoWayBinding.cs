using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using Das.Serializer;

namespace Das.Views.DataBinding
{
    public class TwoWayBinding : SourceBinding
    {
        public TwoWayBinding(INotifyPropertyChanged source,
                             String sourceProperty,
                             IVisualElement target,
                             String targetProperty,
                             IValueConverter? valueConverter,
                             IPropertyAccessor sourcePropertyAccessor)
            : this(source,
                GetObjectPropertyOrDie(source, sourceProperty),
                target,
                GetObjectPropertyOrDie(target, targetProperty), 
                valueConverter, sourcePropertyAccessor) { }

        public TwoWayBinding(INotifyPropertyChanged source,
                             PropertyInfo srcProp,
                             IVisualElement target,
                             PropertyInfo targetProp,
                             IValueConverter? valueConverter,
                             IPropertyAccessor sourcePropertyAccessor)
        : base(source, srcProp, target, targetProp, valueConverter, sourcePropertyAccessor)
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

        protected void SetSourceValue(Object? value)
        {
            var sourceType = _sourceSetter.GetParameters()[0].ParameterType;

            if (value is { } valueValue && !sourceType.IsAssignableFrom(valueValue.GetType()))
                return; //TODO: Can't incompatible types
            
            //if (sourceType == typeof(String) && value is { } validValue &&
            //    validValue.GetType() != typeof(String))
            //    value = validValue.ToString();
            
            _sourceSetter.Invoke(_source, new[] {value});
        }

        private readonly MethodInfo _sourceSetter;
    }
}
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace Das.Views.DataBinding
{
    public class TwoWayBinding : BaseBinding
    {
        public TwoWayBinding(INotifyPropertyChanged source,
                             String sourceProperty,
                             IBindableElement target,
                             String targetProperty)
        {
            _source = source;
            _sourceProperty = sourceProperty;
            _target = target;
            _targetProperty = targetProperty;

            _sourceSetter = source.GetType().GetProperty(sourceProperty)?.GetSetMethod()
                            ?? throw new MissingMethodException(sourceProperty);
            _sourceGetter = source.GetType().GetProperty(sourceProperty)?.GetGetMethod()
                            ?? throw new MissingMethodException(sourceProperty);


            _targetSetter = target.GetType().GetProperty(targetProperty)?.GetSetMethod()
                            ?? throw new MissingMethodException(targetProperty);

            _targetGetter = target.GetType().GetProperty(targetProperty)?.GetGetMethod()
                            ?? throw new MissingMethodException(targetProperty);

            source.PropertyChanged += OnSourcePropertyChanged;
            target.PropertyChanged += OnTargetPropertyChanged;

            SetTargetFromSource();
        }

        public override Object? GetBoundValue(Object? dataContext)
        {
            return _targetGetter.Invoke(_target, new Object[0]);
        }

        public override void Dispose()
        {
            _source.PropertyChanged -= OnSourcePropertyChanged;
            _target.PropertyChanged -= OnTargetPropertyChanged;
        }

        private void OnSourcePropertyChanged(Object sender,
                                             PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _sourceProperty) 
                return;

            SetTargetFromSource();
        }

        private void SetTargetFromSource()
        {
            var val = _sourceGetter.Invoke(_source, new Object[0]);
            _targetSetter.Invoke(_target, new[] {val});
        }

        private void OnTargetPropertyChanged(Object sender,
                                             PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _targetProperty) 
                return;

            var val = _targetGetter.Invoke(_target, new Object[0]);
            _sourceSetter.Invoke(_source, new[] {val});
        }

        private readonly INotifyPropertyChanged _source;
        private readonly String _sourceProperty;
        private readonly MethodInfo _sourceSetter;
        private readonly IBindableElement  _target;
        private readonly MethodInfo _targetGetter;
        private readonly String _targetProperty;
        private readonly MethodInfo _targetSetter;

        private readonly MethodInfo _sourceGetter;
    }
}
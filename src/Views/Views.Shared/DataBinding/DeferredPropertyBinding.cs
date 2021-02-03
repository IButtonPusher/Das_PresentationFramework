using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Mvvm;

namespace Das.Views.DataBinding
{
    public class DeferredPropertyBinding : BaseBinding,
                                           IVisualPropertySetter
    {
        public DeferredPropertyBinding(String sourcePropertyName,
                                       String targetPropertyName,
                                       IPropertyAccessor propertyAccessor,
                                       IValueConverter? converter)
        {
            _propertyAccessor = propertyAccessor;
            SourcePropertyName = sourcePropertyName;
            TargetPropertyName = targetPropertyName;
            Converter = converter;
        }

        public String PropertyName => TargetPropertyName;

        public Object? GetSourceValue(Object? dataContext)
        {
            if (dataContext == null)
                return null;

            return _propertyAccessor.GetPropertyValue(dataContext);
        }

        public IValueConverter? Converter { get; }


        public String SourcePropertyName { get; }

        public String TargetPropertyName { get; }

        public override Object Clone()
        {
            return new DeferredPropertyBinding(SourcePropertyName, TargetPropertyName,
                _propertyAccessor, Converter);
        }

        public override Object? GetBoundValue(Object? dataContext)
        {
            throw new NotImplementedException();
        }


        public override String ToString()
        {
            return SourcePropertyName;
        }

        public override IDataBinding Update(Object? dataContext,
                                            IVisualElement targetVisual)
        {
            try
            {
                var count = Interlocked.Add(ref _updateCounter, 1);


                if (dataContext == null)
                {
                    UpdateDataContext(dataContext);
                    return this;
                }

                if (!(targetVisual is IBindableElement bindingVisual) ||
                    !(TargetPropertyName is { } targetPropertyName))
                    throw new NotImplementedException();


                if (count > 1)
                    return this;


                var sourceProperty = GetObjectPropertyOrDie(dataContext, SourcePropertyName);
                var targetProperty = GetObjectPropertyOrDie(bindingVisual, targetPropertyName);


                var isCollection = typeof(INotifyingCollection).IsAssignableFrom(sourceProperty.PropertyType);

                if (dataContext is INotifyPropertyChanged notifyObject)
                {
                    if (sourceProperty.CanWrite && !isCollection)
                    {
                        var twoWay = new TwoWayBinding(notifyObject, sourceProperty,
                            bindingVisual, targetProperty, Converter, _propertyAccessor);
                        twoWay.Evaluate();
                        return twoWay;
                    }

                    if (isCollection)
                    {
                        var collectionBinding = new OneWayCollectionBinding(notifyObject, sourceProperty,
                            bindingVisual, targetProperty, Converter, _propertyAccessor);
                        collectionBinding.Evaluate();
                        return collectionBinding;
                    }
                }

                // don't do two way if the source property is read only, a collection, or the object 
                // isn't INotifyPropertyChanged
                var oneWay = new SourceBinding(dataContext, sourceProperty, bindingVisual,
                    targetProperty, Converter, _propertyAccessor);
                oneWay.Evaluate();
                return oneWay;
            }
            finally
            {
                Interlocked.Add(ref _updateCounter, -1);
            }
        }

        private readonly IPropertyAccessor _propertyAccessor;

        private Int32 _updateCounter;
    }
}

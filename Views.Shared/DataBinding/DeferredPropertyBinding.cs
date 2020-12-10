using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Mvvm;
using Das.Views.Rendering;

namespace Das.Views.DataBinding
{
    public class DeferredPropertyBinding : BaseBinding,
                                           IVisualPropertySetter
    {
        public DeferredPropertyBinding(String sourcePropertyName,
                                       String targetPropertyName,
                                       IValueConverter? converter = null)
            //: this(sourcePropertyName)
        {
            SourcePropertyName = sourcePropertyName;
            TargetPropertyName = targetPropertyName;
            Converter = converter;
        }


        public String SourcePropertyName { get; }

        public String TargetPropertyName { get; }
        
        public IValueConverter? Converter { get; }

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

                if (targetPropertyName == nameof(IBindableElement.DataContext))
                {

                }

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
                            bindingVisual, targetProperty, Converter);
                        twoWay.Evaluate();
                        return twoWay;
                    }

                    if (isCollection)
                    {
                        var collectionBinding = new OneWayCollectionBinding(notifyObject, sourceProperty,
                            bindingVisual, targetProperty, Converter);
                        collectionBinding.Evaluate();
                        return collectionBinding;
                    }

                }

                // don't do two way if the source property is read only, a collection, or the object 
                // isn't INotifyPropertyChanged
                var oneWay = new SourceBinding(dataContext, sourceProperty, bindingVisual, 
                    targetProperty, Converter);
                oneWay.Evaluate();
                return oneWay;
            }
            finally
            {
                Interlocked.Add(ref _updateCounter, -1);
            }
        }

        public override Object Clone()
        {
            return new DeferredPropertyBinding(SourcePropertyName, TargetPropertyName,
                Converter);
        }

        private Int32 _updateCounter;

        public String PropertyName => TargetPropertyName;

        public Object? GetSourceValue(Object? dataContext)
        {
            if (dataContext == null)
                return null;

            var prop = dataContext.GetType().GetProperty(SourcePropertyName);
            if (prop == null)
                return null;
            return prop.GetValue(dataContext, null);
        }
    }
}
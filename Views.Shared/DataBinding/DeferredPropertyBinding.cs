using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using Das.Views.Mvvm;
using Das.Views.Rendering;

namespace Das.Views.DataBinding
{
    public class DeferredPropertyBinding<T> : BaseBinding<T>
    {
        // ReSharper disable once UnusedMember.Global
        public DeferredPropertyBinding(Type parentClassType,
                                       String propertyName)
        {
            _prop = parentClassType.GetProperty(propertyName) ??
                    throw new MissingMemberException(parentClassType.Name, propertyName);
        }

        public DeferredPropertyBinding(PropertyInfo prop)
        {
            _prop = prop;
        }


        public override IDataBinding<T> DeepCopy()
        {
            return new DeferredPropertyBinding<T>(_prop);
        }

        public override T GetValue(Object? dataContext)
        {
            if (_prop.GetValue(dataContext, null) is T prop)
                return prop;
            return default!;
        }

        //public override IDataBinding ToSingleBinding()
        //{
        //    var argh = _prop.PropertyType.GetGenericArguments().First();
        //    var itemType = typeof(InstanceBinding<>).MakeGenericType(argh);
        //    return (IDataBinding) Activator.CreateInstance(itemType, null);
        //}

        private readonly PropertyInfo _prop;

    }

    public class DeferredPropertyBinding : BaseBinding
    {
        public String SourcePropertyName { get; }

        public String? TargetPropertyName { get; }

        public DeferredPropertyBinding(String sourcePropertyName,
                                       String targetPropertyName)
            : this(sourcePropertyName)
        {
            TargetPropertyName = targetPropertyName;
        }

        public DeferredPropertyBinding(String sourcePropertyName)
        {
            SourcePropertyName = sourcePropertyName;
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
            if (dataContext == null)
            {
                UpdateDataContext(dataContext);
                return this;
            }

            if (dataContext is INotifyPropertyChanged notifyObject && 
                targetVisual is IBindableElement bindingVisual && 
                TargetPropertyName is {} targetPropertyName)
            {
                var sourceProperty = GetObjectPropertyOrDie(dataContext, SourcePropertyName);
                var targetProperty = GetObjectPropertyOrDie(bindingVisual, targetPropertyName);

                var isCollection = typeof(INotifyingCollection).IsAssignableFrom(sourceProperty.PropertyType);

                if (sourceProperty.CanWrite && !isCollection)
                {
                    var twoWay = new TwoWayBinding(notifyObject, sourceProperty, bindingVisual, targetProperty);
                    twoWay.Evaluate();
                    return twoWay;
                }

                if (isCollection)
                {
                    var collectionBinding = new OneWayCollectionBinding(notifyObject, sourceProperty, 
                        bindingVisual, targetProperty);
                    collectionBinding.Evaluate();
                    return collectionBinding;
                }

                //don't do two way if the source property is read only or if it's a collection
                var oneWay = new SourceBinding(notifyObject, sourceProperty, bindingVisual, targetProperty);
                oneWay.Evaluate();
                return oneWay;
            }

            throw new NotImplementedException();

            return base.Update(dataContext, targetVisual);
        }
    }
}
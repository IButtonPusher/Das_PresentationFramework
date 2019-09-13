using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Das.Serializer;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.DevKit
{
    // ReSharper disable once ClassNeverInstantiated.Global - via de-serialized
    public class ViewBuilder : IView, IBindingSetter
    {
        void IVisualRenderer.Arrange(ISize availableSpace, IRenderContext renderContext)
        {
            renderContext.DrawElement(Content, new Rectangle(0,0,availableSpace));
        }

        ISize IVisualRenderer.Measure(ISize availableSpace, IMeasureContext measureContext)
            => measureContext.MeasureElement(Content, availableSpace);

        [IgnoreDataMember]
        public ISerializationCore Serializer { get; set; }
        

        IVisualElement IDeepCopyable<IVisualElement>.DeepCopy() => null;

        int IVisualElement.Id => -1;

        public IVisualElement Content { get; set; }

        public String DesignObject { get; set; }

        public String Binding { get; set; }

        public IStyleContext StyleContext { get; set; }

        public void SetBoundValue(object value)
        {
            var viewBindingType = Serializer.GetTypeFromClearName(Binding);

            _viewModel = value as IViewModel;

            if (!(Content is IBindableElement setter))
                return;

            SetDataContext(setter, viewBindingType);

            setter.SetDataContext(value);

            _isChanged = true;
        }

        public void SetDataContext(object dataContext) => SetBoundValue(dataContext);

        private void SetDataContext(IBindableElement element, Type parentType)
        {
            while (true)
            {
                var rType = GetPropertyBinding(element, parentType);
                if (rType == null)
                    throw new InvalidOperationException($"Invalid binding on element: {element}");
                var _ = SetDataBinding(element, parentType, rType);

                switch (element)
                {
                    case IRepeaterPanel repeater when repeater.Content is IBindableElement bindable:
                        var repeatingType = Serializer.GetGermaneType(rType.PropertyType);
                        SetDataBinding(bindable, repeatingType, rType);

                        if (bindable is IVisualContainer cnt)
                        {
                            foreach (var child in cnt.Children.OfType<IBindableElement>()) SetDataContext(child, repeatingType);
                        }

                        break;

                    case IContentContainer content:
                        if (content.Content is IBindableElement bindableContent)
                        {
                            element = bindableContent;
                            continue;
                        }

                        break;
                    case IVisualContainer container:
                        foreach (var child in container.Children.OfType<IBindableElement>())
                        {
                            SetDataContext(child, parentType);
                        }

                        break;
                    case IBindingSetter _:
                        break;
                    default:
                        throw new NotImplementedException();
                }

                break;
            }
        }

        private PropertyInfo GetPropertyBinding(IBindableElement element, 
            Type parentType)
        {
            var strBinding = element.Binding?.ToString();
            if (strBinding == null)
                return default;

            var propInfo = Serializer.FindPublicProperty(parentType, strBinding);

            return propInfo;
        }

        private IDataBinding SetDataBinding(IBindableElement element, Type parentType, 
            PropertyInfo bindingProp)
        {
            var elementType = element.GetType();
            var strBinding = element.Binding?.ToString();

            Type genericArg;
            PropertyInfo propInfo;

            if (strBinding == null)
            {
                genericArg = parentType;
                propInfo = bindingProp;
            }
            else
            {
                propInfo = Serializer.FindPublicProperty(parentType, strBinding);
                genericArg = Serializer.GetPropertyType(elementType, "Binding").GenericTypeArguments[0];
            }
            var genericBindingType = typeof(DeferredPropertyBinding<>).MakeGenericType(genericArg);

            var cookedBinding = (IDataBinding)Activator.CreateInstance(genericBindingType, propInfo);
            element.Binding = cookedBinding;
           

            return cookedBinding;
        }

        public void AcceptChanges()
        {
            _isChanged = false;
        }

        private Boolean _isChanged;
        public bool IsChanged => _isChanged || _viewModel?.IsChanged == true;

        private IViewModel _viewModel;
    }
}

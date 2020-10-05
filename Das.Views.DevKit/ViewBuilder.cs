﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable 8618

namespace Das.Views.DevKit
{
    // ReSharper disable once ClassNeverInstantiated.Global - via de-serialized
    public class ViewBuilder : IView
    {
        void IVisualRenderer.Arrange(ISize availableSpace,
                                     IRenderContext renderContext)
        {
            if (Content is {} content)
                renderContext.DrawElement(content, new Rectangle(0, 0, availableSpace));
        }

        ISize IVisualRenderer.Measure(ISize availableSpace, IMeasureContext measureContext)
        {
            if (Content is {} content)
                return measureContext.MeasureElement(content, availableSpace);

            return Size.Empty;
        }

        [IgnoreDataMember]
        public ISerializationCore Serializer { get; set; }
        

        IVisualElement IDeepCopyable<IVisualElement>.DeepCopy() => null!;

        Int32 IVisualElement.Id => -1;

        public IVisualElement? Content { get; set; }

        public String DesignObject { get; set; }

        public String Binding { get; set; }

        public IStyleContext StyleContext { get; set; }

        public void SetBoundValue(Object? value)
        {
            var viewBindingType = Serializer.TypeInferrer.GetTypeFromClearName(Binding);

            _viewModel = value;// as IMutableVm;

            if (!(Content is IBindableElement setter))
                return;

            if (viewBindingType != null)
                SetDataContext(setter, viewBindingType);

            setter.SetDataContext(value);

            _isChanged = true;
        }

        public Task SetBoundValueAsync(Object? value)
        {
            SetBoundValue(value);
            return Task.CompletedTask;
        }

        public void SetDataContext(Object? dataContext) => SetBoundValue(dataContext);

        public Task SetDataContextAsync(Object? dataContext)
        {
            SetDataContext(dataContext);
            return Task.CompletedTask;
        }

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
                        var repeatingType = Serializer.TypeInferrer.GetGermaneType(rType.PropertyType);
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

        private PropertyInfo? GetPropertyBinding(IBindableElement element, 
                                                 Type parentType)
        {
            var strBinding = element.Binding?.ToString();
            if (strBinding == null)
                return default;

            var propInfo = Serializer.TypeManipulator.FindPublicProperty(parentType, strBinding);

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
                propInfo = Serializer.TypeManipulator.FindPublicProperty(parentType, strBinding);
                genericArg = Serializer.TypeManipulator.GetPropertyType(elementType, "Binding").GenericTypeArguments[0];
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

        public Boolean IsChanged
        {
            get
            {
                if (_isChanged)
                    return true;

                if (_viewModel is IChangeTracking changeTracking &&
                    changeTracking.IsChanged)
                    return true;

                return Content is IChangeTracking content && content.IsChanged;
            }
        }

        //private IMutableVm? _viewModel;
        private Object? _viewModel;

        public void Dispose()
        {
            if (_viewModel is IDisposable disposable)
                disposable.Dispose();
            // _viewModel?.Dispose();
            Content?.Dispose();
        }
    }
}

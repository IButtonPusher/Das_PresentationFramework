﻿using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Controls;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Templates;
// ReSharper disable All

// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable 8618

namespace Das.Views.DevKit
{
    // ReSharper disable once ClassNeverInstantiated.Global - via de-serialized
    public class ViewBuilder : ContentPanel, IView
    {
        
        
        void IVisualRenderer.Arrange<TRenderSize>(TRenderSize availableSpace,
                                                  IRenderContext renderContext)
        {
            if (Content is {} content)
                renderContext.DrawElement(content, new RenderRectangle(0, 0, 
                    availableSpace.Width, availableSpace.Height, availableSpace.Offset));
        }

        ValueSize IVisualRenderer.Measure<TRenderSize>(TRenderSize availableSpace, 
                                                      IMeasureContext measureContext)
        {
            if (Content is {} content)
                return measureContext.MeasureElement(content, availableSpace);

            return ValueSize.Empty;
        }

        public override void InvalidateMeasure()
        {
            Content?.InvalidateMeasure();
        }

        public override void InvalidateArrange()
        {
            Content?.InvalidateArrange();
        }

        //public Boolean IsRequiresMeasure => Content!.IsRequiresMeasure;

        //public Boolean IsRequiresArrange => Content!.IsRequiresArrange;

        [IgnoreDataMember]
        public ISerializationCore Serializer { get; set; }
        

        //IVisualElement IDeepCopyable<IVisualElement>.DeepCopy() => null!;

        Int32 IVisualElement.Id => -1;

        //public Boolean IsClipsContent { get; set; }

        //public event Action<IVisualElement>? Disposed;

        IVisualTemplate? ITemplatableVisual.Template
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        //public void AcceptChanges(ChangeType changeType)
        //{
            
        //}

        //public void RaisePropertyChanged(String propertyName, 
        //                                 Object? value)
        //{
            
        //}

        //public void RaisePropertyChanged(String propertyName)
        //{
            
        //}

        //public Double? Width { get; set; }

        //public Double? Height { get; set; }

        //public HorizontalAlignments HorizontalAlignment
        //{
        //    get => Content?.HorizontalAlignment ?? HorizontalAlignments.Stretch;
        //    set
        //    {
        //        if (Content is {} content)
        //            content.HorizontalAlignment = value;
        //    }
        //}

        //public VerticalAlignments VerticalAlignment
        //{
        //    get => Content?.VerticalAlignment ?? VerticalAlignments.Stretch;
        //    set
        //    {
        //        if (Content is {} content)
        //            content.VerticalAlignment = value;
        //    }
        //}

        //public IBrush? Background
        //{
        //    get => Content?.Background;
        //    set
        //    {
        //        if (Content  is {} content)
        //            content.Background = value;
        //    }
        //}

        //public Thickness? Margin
        //{
        //    get => Content?.Margin;
        //    set => Content!.Margin = value;
        //}

        //public void OnParentChanging(IContainerVisual? newParent)
        //{
            
        //}

        //public IVisualElement? Content { get; set; }

        public String DesignObject { get; set; }

        public String Binding { get; set; }

        //public IStyleContext StyleContext { get; set; }

        //public void SetBoundValue(Object? value)
        //{
        //    var viewBindingType = Serializer.TypeInferrer.GetTypeFromClearName(Binding);

        //    _viewModel = value;

        //    if (!(Content is IBindableElement setter))
        //        return;

        //    if (viewBindingType != null)
        //        SetDataContext(setter, viewBindingType, value);

        //    setter.SetDataContext(value);

        //    _isChanged = true;
        //}

        //public Task SetBoundValueAsync(Object? value)
        //{
        //    SetBoundValue(value);
        //    return Task.CompletedTask;
        //}

        //public void SetDataContext(Object? dataContext) => SetBoundValue(dataContext);

        //public Task SetDataContextAsync(Object? dataContext)
        //{
        //    SetDataContext(dataContext);
        //    return Task.CompletedTask;
        //}

        //private void SetDataContext(IBindableElement element, 
        //                            Type parentType,
        //                            Object? parentBinding)
        //{
        //    while (true)
        //    {
        //        var rType = GetPropertyBinding(element, parentType);
        //        if (rType == null)
        //        {
        //            throw new InvalidOperationException($"Invalid binding on element: {element}");
        //        }

        //        var _ = SetDataBinding(element, parentType, rType);

        //        switch (element)
        //        {
        //            case IRepeaterPanel repeater when repeater.Content is IBindableElement bindable:
        //                var repeatingType = Serializer.TypeInferrer.GetGermaneType(rType.PropertyType);
        //                SetDataBinding(bindable, repeatingType, rType);

        //                if (bindable is IVisualContainer cnt)
        //                {
        //                    cnt.Children.RunOnEachChild(c =>
        //                    {
        //                        if (c is IBindableElement child)
        //                            SetDataContext(child, repeatingType, parentBinding);
        //                    });

        //                    //foreach (var child in cnt.Children.OfType<IBindableElement>())
        //                    //{
        //                    //    SetDataContext(child, repeatingType, parentBinding);
        //                    //}
        //                }

        //                break;

        //            case IContentContainer content:
        //                if (content.Content is IBindableElement bindableContent)
        //                {
        //                    element = bindableContent;
        //                    continue;
        //                }

        //                break;
        //            case IVisualContainer container:
        //                container.Children.RunOnEachChild(c =>
        //                {
        //                    if (c is IBindableElement child)
        //                        SetDataContext(child, parentType, parentBinding);
        //                });
        //                //foreach (var child in container.Children.OfType<IBindableElement>())
        //                //{
        //                //    SetDataContext(child, parentType, parentBinding);
        //                //}

        //                break;
        //            case IBindingSetter _:
        //                break;
        //            default:
        //                throw new NotImplementedException();
        //        }

        //        break;
        //    }
        //}

        //private PropertyInfo? GetPropertyBinding(IBindableElement element, 
        //                                         Type parentType)
        //{
        //    var strBinding = element.Binding?.ToString();
        //    if (strBinding == null)
        //        return default;

        //    var propInfo = Serializer.TypeManipulator.FindPublicProperty(parentType, strBinding);

        //    return propInfo;
        //}

        //private IDataBinding SetDataBinding(IBindableElement element,
        //                                    Type parentType,
        //                                    PropertyInfo bindingProp)
        //{
        //    var elementType = element.GetType();
        //    var strBinding = element.Binding?.ToString();

        //    Type? genericArg;
        //    PropertyInfo? propInfo;

        //    if (strBinding == null)
        //    {
        //        genericArg = parentType;
        //        propInfo = bindingProp;
        //    }
        //    else
        //    {
        //        propInfo = Serializer.TypeManipulator.FindPublicProperty(parentType, strBinding);
        //        var propType = Serializer.TypeManipulator.GetPropertyType(elementType, "Binding");

        //        if (propType == null)
        //            throw new InvalidOperationException();

        //        genericArg = propType.GenericTypeArguments[0];
        //    }

        //    var genericBindingType = typeof(DeferredPropertyBinding<>).MakeGenericType(genericArg);

        //    var cookedBinding = (IDataBinding) Activator.CreateInstance(genericBindingType, propInfo);
        //    element.Binding = cookedBinding;


        //    return cookedBinding;
        //}

        //public void AcceptChanges()
        //{
        //    _isChanged = false;
        //}

        //private Boolean _isChanged;

        //public Boolean IsChanged
        //{
        //    get
        //    {
        //        if (_isChanged)
        //            return true;

        //        if (_viewModel is IChangeTracking changeTracking &&
        //            changeTracking.IsChanged)
        //            return true;

        //        return Content is IChangeTracking content && content.IsChanged;
        //    }
        //}

        //private IMutableVm? _viewModel;
        private Object? _viewModel;

        public                 override  void Dispose()
        {
            
            if (_viewModel is IDisposable disposable)
                disposable.Dispose();
            // _viewModel?.Dispose();
            Content?.Dispose();
            base.Dispose();
            //Disposed?.Invoke(this);
            //Disposed = null;
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        // ReSharper disable once UnusedMember.Global
        //protected virtual void OnPropertyChanged([CallerMemberName] String? propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        //public Boolean Equals(IVisualElement other)
        //{
        //    return ReferenceEquals(this, other);
        //}

        public ViewBuilder(IVisualBootstrapper visualBootstrapper) : base(visualBootstrapper)
        {
            _viewModel = null;
        }
    }
}

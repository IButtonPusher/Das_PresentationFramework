using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using Common.Core;

namespace Das.ViewModels.ChangeTracking;

public abstract class ChangeTrackingVmHelperEditVmHelper<T> : IDisposable //: BaseViewModel,
                                                     //IChangeTracking
    where T : BaseViewModel, IEditorChange
{

    private static readonly ConcurrentDictionary<T, HashSet<String>> _changedProperties;
    private static readonly ConcurrentDictionary<T, Dictionary<String, Object?>> _propertyValues;
    private readonly Object _synch;

    static ChangeTrackingVmHelperEditVmHelper()
    {
        _changedProperties = new ConcurrentDictionary<T, HashSet<String>>();
        _propertyValues = new ConcurrentDictionary<T, Dictionary<String, Object?>>();

        
    }


    public ChangeTrackingVmHelperEditVmHelper(T viewModel,
                                              Func<T, String, Object?> getPropertyValue,
                                              IReadOnlyCollection<String> triggeringPropertyNames)
    {
        _synch = new Object();

        _viewModel = viewModel;
        _getPropertyValue = getPropertyValue;
        //_editPropertyNames = new List<String>();
        //_propertyValues = new Dictionary<String, Object?>();
        //_changedProperties = new HashSet<String>();


        _editPropertyNames ??= new HashSet<String>(triggeringPropertyNames);
        //foreach (var propName in triggeringPropertyNames)
        //{
        //    _editPropertyNames.Add(propName);
        //}

        if (!_propertyValues.TryAdd(viewModel, new Dictionary<String, Object?>()))
            return;

        _changedProperties.TryAdd(viewModel, new HashSet<String>());

        AcceptChanges();

        viewModel.PropertyChanged += OnViewModelPropertyChanged;

        

    }

    public ChangeTrackingVmHelperEditVmHelper(T viewModel,
                                              Func<Type, IEnumerable<PropertyInfo>> getPublicProperties,
                                              Func<T, String, Object?> getPropertyValue)
        : this(viewModel, getPropertyValue, 
            new List<String>(EnumerateTriggeringPropertyNames(typeof(T), getPublicProperties)))

    {
        //var changeType = ChangeTrackingTypes.SpecifyTriggeringProperties;


        //_viewModel = viewModel;
        //_getPropertyValue = getPropertyValue;
        //_editPropertyNames = new List<String>();
        //_propertyValues = new Dictionary<String, Object?>();
        //_changedProperties = new HashSet<String>();

        //var vmType = typeof(T);
        //if (vmType.GetCustomAttribute<ChangeTrackingTypeAttribute>() is { } attr)
        //{
        //    changeType = attr.TrackingType;
        //}

       

        ////_typeStructure = typeCore.GetTypeStructure<T>();

        //foreach (var prop in getPublicProperties(vmType))
        //{
        //    if (!prop.CanWrite)
        //        continue;

        //    switch (changeType)
        //    {
        //        case ChangeTrackingTypes.SpecifyTriggeringProperties when 
        //            prop.GetCustomAttribute<TriggersIsChangedAttribute>(true) == null:
        //            continue;
                
        //        case ChangeTrackingTypes.SpecifyExcludedProperties when
        //            prop.GetCustomAttribute<TriggersIsChangedAttribute>(true) != null:
        //            //TryGetCustomAttribute<NonEditPropertyAttribute>(prop, out _):
        //            continue;
                
        //    }

        //    //if (TryGetCustomAttribute<NonEditPropertyAttribute>(prop, out _))
        //    //    continue;

        //    _editPropertyNames.Add(prop.Name);
        //}

        //AcceptChanges();

        //viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private static IEnumerable<String> EnumerateTriggeringPropertyNames(Type vmType,
                                                                  Func<Type, IEnumerable<PropertyInfo>> getPublicProperties)
    {
        var changeType = ChangeTrackingTypes.SpecifyTriggeringProperties;

        if (vmType.GetCustomAttribute<ChangeTrackingTypeAttribute>() is { } attr)
            changeType = attr.TrackingType;
        
        foreach (var prop in getPublicProperties(vmType))
        {
            if (!prop.CanWrite)
                continue;

            switch (changeType)
            {
                case ChangeTrackingTypes.SpecifyTriggeringProperties when 
                    prop.GetCustomAttribute<TriggersIsChangedAttribute>(true) == null:
                    continue;
                
                case ChangeTrackingTypes.SpecifyExcludedProperties when
                    prop.GetCustomAttribute<NonEditPropertyAttribute>(true) != null:
                    //TryGetCustomAttribute<NonEditPropertyAttribute>(prop, out _):
                    continue;
                
            }

            //if (TryGetCustomAttribute<NonEditPropertyAttribute>(prop, out _))
            //    continue;

            yield return prop.Name;
        }
    }

    public void AcceptChanges()
    {
        var vm = _viewModel;

        if (!_propertyValues.TryGetValue(vm, out var propVals) || 
            !_changedProperties.TryGetValue(vm, out var changedProps))
        {
            return;
        }

        lock (_synch)
        {
            foreach (var propName in _editPropertyNames!)
                propVals[propName] = _getPropertyValue(vm, propName);

            changedProps.Clear();

            vm.UpdateIsChanged(false);
        }
    }

    //[NonEditPropertyAttribute]
    //public Boolean IsChanged
    //{
    //    get => _isChanged;
    //    set => SetValue(ref _isChanged, value, OnHasChangesChanged);
    //}


    public IReadOnlyCollection<String> TriggeringPropertyNames => _editPropertyNames!;

    public void Dispose()
    {
        _propertyValues.TryRemove(_viewModel, out _);
        _changedProperties.TryRemove(_viewModel, out _);

        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    //protected static Boolean TryGetCustomAttribute<TAttribute>(MemberInfo m,
    //                                                           out TAttribute value)
    //    where TAttribute : Attribute
    //{
    //    Object[] customAttributes = m.GetCustomAttributes(typeof(TAttribute), true);
    //    if (customAttributes.Length == 0)
    //    {
    //        value = default!;
    //        return false;
    //    }

    //    value = (customAttributes[0] as TAttribute)!;
    //    return value != null;
    //}

    protected virtual void OnHasChangesChanged(Boolean newValue)
    {
    }

    private void OnViewModelPropertyChanged(Object? sender,
                                            PropertyChangedEventArgs e)
    {
        var vm = _viewModel;

        if (e.PropertyName is not { } propertyName || 
            !_editPropertyNames!.Contains(propertyName))
            return;

        if (!_propertyValues.TryGetValue(vm, out var propVals) ||
            !_changedProperties.TryGetValue(vm, out var changedProps))
            return;

        lock (_synch)
        {
            if (!propVals.TryGetValue(propertyName, out var propVal))
                return;

            if (Equals(propVal, _getPropertyValue(vm, propertyName)))
                changedProps.Remove(propertyName);
            else
                changedProps.Add(propertyName);

            vm.UpdateIsChanged(changedProps.Count > 0);
        }

      
    }

    //private readonly HashSet<String> _changedProperties;
    private static HashSet<String>? _editPropertyNames;
    private readonly Func<T, String, Object?> _getPropertyValue;

    //private readonly Dictionary<String, Object?> _propertyValues;

    //private readonly ITypeStructure _typeStructure;
    private readonly T _viewModel;

}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace Das.ViewModels.ChangeTracking;

public abstract class ChangeTrackingVmHelperEditVmHelper<T> : IDisposable
    where T : BaseViewModel, IEditorChange
{
    static ChangeTrackingVmHelperEditVmHelper()
    {
        _changedProperties = new ConcurrentDictionary<T, HashSet<String>>();
        _propertyValues = new ConcurrentDictionary<T, Dictionary<String, Object?>>();
    }


    protected ChangeTrackingVmHelperEditVmHelper(T viewModel,
                                                 Func<T, String, Object?> getPropertyValue,
                                                 IEnumerable<KeyValuePair<String, Boolean>> triggeringPropertyNames)
    {
        _synch = new Object();

        _viewModel = viewModel;
        _getPropertyValue = getPropertyValue;

        if (_editPropertyNames == null)
        {
            var editPropertyNames = new Dictionary<String, Boolean>();
            foreach (var tpn in triggeringPropertyNames)
                editPropertyNames.Add(tpn.Key, tpn.Value);

            _editPropertyNames = editPropertyNames;
        }

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
            EnumerateTriggeringPropertyNames(typeof(T), getPublicProperties))
    {
    }

    public void Dispose()
    {
        _propertyValues.TryRemove(_viewModel, out _);
        _changedProperties.TryRemove(_viewModel, out _);

        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    public void AcceptChanges()
    {
        if (!_propertyValues.TryGetValue(_viewModel, out var propVals) ||
            !_changedProperties.TryGetValue(_viewModel, out var changedProps))
        {
            return;
        }

        lock (_synch)
        {
            foreach (var propName in _editPropertyNames!)
            {
                propVals[propName.Key] = _getPropertyValue(_viewModel, propName.Key);
            }

            changedProps.Clear();

            _viewModel.UpdateIsChanged(false);
        }
    }

    private static IEnumerable<KeyValuePair<String, Boolean>> EnumerateTriggeringPropertyNames(Type vmType,
        Func<Type, IEnumerable<PropertyInfo>> getPublicProperties)
    {
        var changeType = ChangeTrackingTypes.SpecifyTriggeringProperties;

        if (vmType.GetCustomAttribute<ChangeTrackingTypeAttribute>() is { } attr)
            changeType = attr.TrackingType;

        switch (changeType)
        {
            case ChangeTrackingTypes.SpecifyTriggeringProperties:
                foreach (var prop in getPublicProperties(vmType))
                {
                    var triggerChange = prop.GetCustomAttribute<TriggersIsChangedAttribute>(true);
                    if (triggerChange != null)
                        yield return new KeyValuePair<String, Boolean>(prop.Name, triggerChange.CheckEquality);
                }

                break;

            case ChangeTrackingTypes.SpecifyExcludedProperties:
                foreach (var prop in getPublicProperties(vmType))
                {
                    if (!prop.CanWrite ||
                        prop.GetCustomAttribute<NonEditPropertyAttribute>(true) != null)
                        continue;

                    yield return new KeyValuePair<String, Boolean>(prop.Name, true);
                }

                break;
        }
    }

    //protected virtual void OnHasChangesChanged(Boolean newValue)
    //{
    //}

    private void OnViewModelPropertyChanged(Object? sender,
                                            PropertyChangedEventArgs e)
    {
        var vm = _viewModel;

        if (e.PropertyName is not { } propertyName ||
            !_editPropertyNames!.TryGetValue(propertyName, out var checkEquality))
            return;

        if (!_propertyValues.TryGetValue(vm, out var propVals) ||
            !_changedProperties.TryGetValue(vm, out var changedProps))
            return;

        lock (_synch)
        {
            if (!propVals.TryGetValue(propertyName, out var propVal))
                return;

            if (checkEquality && Equals(propVal, _getPropertyValue(vm, propertyName)))
                changedProps.Remove(propertyName);
            else
                changedProps.Add(propertyName);

            vm.UpdateIsChanged(changedProps.Count > 0);
        }
    }


    public IReadOnlyDictionary<String, Boolean> TriggeringPropertyNames => _editPropertyNames!;

    private static readonly ConcurrentDictionary<T, HashSet<String>> _changedProperties;
    private static readonly ConcurrentDictionary<T, Dictionary<String, Object?>> _propertyValues;

    private static Dictionary<String, Boolean>? _editPropertyNames;
    private readonly Func<T, String, Object?> _getPropertyValue;
    private readonly Object _synch;

    private readonly T _viewModel;
}

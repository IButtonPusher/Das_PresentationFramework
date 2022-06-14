using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Common.Core;

namespace Das.ViewModels.NotifyProperties
{
    public abstract class ChangeTrackingVm : BaseViewModel,
                                             IChangeTracking
    {
        public ChangeTrackingVm()
        {
            _propertyValues = new ConcurrentDictionary<String, Object?>();
            _properties = new Dictionary<String, PropertyInfo>();

            _modifiedPropertiesLock = new Object();
            _modifiedProperties = new HashSet<String>();

            var myType = GetType();
            if (myType == null)
                return;

            var allMyProps = myType.GetProperties(BindingFlags.Instance | BindingFlags.FlattenHierarchy
                                                       | BindingFlags.Public
                                                       | BindingFlags.NonPublic);


            foreach (var prop in allMyProps)
            {
                if (prop.GetCustomAttribute<TriggersIsChangedAttribute>() == default)
                    continue;

                _propertyValues[prop.Name] = default;
                _properties[prop.Name] = prop;
            }
        }


        [DebuggerStepThrough]
        [DebuggerHidden]
        protected override void RaisePropertyChanged(String propertyName)
        {
            if (_propertyValues.TryGetValue(propertyName, out var propVal) && 
                _properties.TryGetValue(propertyName, out var prop))
            {
                var nowVal = prop.GetValue(this);
                if (Equals(propVal, nowVal))
                {
                    lock (_modifiedPropertiesLock)
                    {
                        if (_modifiedProperties.Remove(propertyName) &&
                            _modifiedProperties.Count == 0)
                            IsChanged = false;
                    }
                }
                else
                {
                    lock (_modifiedPropertiesLock)
                    {
                        if (_modifiedProperties.Add(propertyName) &&
                            _modifiedProperties.Count == 0)
                            IsChanged = true;
                    }
                }
            }

            base.RaisePropertyChanged(propertyName);
        }

        private readonly ConcurrentDictionary<String, Object?> _propertyValues;
        private readonly HashSet<String> _modifiedProperties;
        private readonly Object _modifiedPropertiesLock;
        private readonly Dictionary<String, PropertyInfo> _properties;

        public virtual void AcceptChanges()
        {
            foreach (var k in _propertyValues.Keys)
            {
                _propertyValues[k] = _properties[k].GetValue(this);
            }

            IsChanged = false;
        }


        private Boolean _isChanged;

        public Boolean IsChanged
        {
            get => _isChanged;
            set => SetValue(ref _isChanged, value);
        }
    }
}

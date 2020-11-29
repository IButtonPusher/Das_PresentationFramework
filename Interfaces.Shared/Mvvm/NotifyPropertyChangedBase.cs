using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

// ReSharper disable UnusedMember.Global
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace Das.Views.Mvvm
{
    public abstract class NotifyPropertyChangedBase : IViewModel
    {
        public virtual event PropertyChangedEventHandler? PropertyChanged;

        public virtual void Dispose()
        {
            PropertyChanged = null;
            PropertyChanging = null;
            PropertyValueChanged = null;
        }

        /// <summary>
        ///     Sender, Property Name, Old Value, New Value, allow change
        /// </summary>
        public event Func<Object, String, Object, Object, Boolean>? PropertyChanging;

        public event Action<String, Object?>? PropertyValueChanged;

        protected virtual void RaisePropertyChanged(String propertyName,
                                                    Object? newValue)
        {
            if (PropertyValueChanged is { } propValChanged)
                propValChanged(propertyName, newValue);

            RaisePropertyChanged(propertyName);
        }

        protected virtual void RaisePropertyChanged(String propertyName)
        {
            var handler = PropertyChanged;
            if (handler == null)
                return;

            var args = new PropertyChangedEventArgs(propertyName);
            handler(this, args);
        }

        // ReSharper disable once UnusedMember.Global
        protected void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        protected virtual Boolean SetValue<T>(ref T field,
                                              T value,
                                              [CallerMemberName] String? propertyName = null)
        {
            if (propertyName != null && !VerifyCanChangeValue(field, value, null, propertyName))
                return false;

            SetValueImpl(ref field, value, null, propertyName);
            return true;
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        protected virtual void SetValue<T>(ref T field,
                                           T newValue,
                                           Func<T, Task> handleValueChanged,
                                           [CallerMemberName] String propertyName = "")
        {
            if (!VerifyCanChangeValue(field, newValue, null, propertyName))
                return;

            SetValueImpl(ref field, newValue, handleValueChanged, propertyName);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        protected virtual void SetValue<T>(ref T field,
                                           T newValue,
                                           Func<T, T, Boolean> onValueChanging,
                                           [CallerMemberName] String propertyName = "")
        {
            if (!VerifyCanChangeValue(field, newValue, onValueChanging, propertyName))
                return;

            //if (!onValueChanging(field, newValue))
            //    return;

            SetValueImpl(ref field, newValue, null, propertyName);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        protected virtual void SetValue<T>(ref T field,
                                           T newValue,
                                           Func<T, T, Boolean> onValueChanging,
                                           Action<T> handleValueChanged,
                                           [CallerMemberName] String propertyName = "")
        {
            if (!VerifyCanChangeValue(field, newValue, onValueChanging, propertyName))
                return;

            //if (!onValueChanging(field, newValue))
            //    return;

            SetValueImpl(ref field, newValue, handleValueChanged, propertyName);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        protected virtual void SetValue<T>(ref T field,
                                           T newValue,
                                           Func<T, T, Boolean> onValueChanging,
                                           Func<T, Task> handleValueChangedAsync,
                                           [CallerMemberName] String propertyName = "")
        {
            if (!VerifyCanChangeValue(field, newValue, onValueChanging, propertyName))
                return;

            //if (!onValueChanging(field, newValue))
            //    return;

            SetValueImpl(ref field, newValue, handleValueChangedAsync, propertyName);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        protected virtual Boolean SetValue<T>(ref T field,
                                              T newValue,
                                              Action<T> onValueChanged,
                                              [CallerMemberName] String propertyName = "")
        {
            if (!VerifyCanChangeValue(field, newValue, null, propertyName))
                return false;

            SetValueImpl(ref field, newValue, onValueChanged, propertyName);
            return true;
            //if (!SetValue(ref field, value, propertyName))
            //    return false;
            //onValueChanged(value);
            //return true;
        }

        // ReSharper disable once UnusedMember.Global
        protected Boolean TryGetPropertyHandler(out PropertyChangedEventHandler handler)
        {
            handler = PropertyChanged!;
            return handler != null!;
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        // ReSharper disable once RedundantAssignment
        private void SetValueImpl<T>(ref T field,
                                     T value,
                                     Func<T, Task>? handleValueChanged,
                                     [CallerMemberName] String? propertyName = null)
        {
            field = value;

            if (handleValueChanged is { } validHandler)
                validHandler(value);

            if (propertyName != null)
                RaisePropertyChanged(propertyName, value);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        // ReSharper disable once RedundantAssignment
        private void SetValueImpl<T>(ref T field,
                                     T value,
                                     Action<T>? handleValueChanged,
                                     [CallerMemberName] String? propertyName = null)
        {
            field = value;

            if (handleValueChanged is { } validHandler)
                validHandler(value);

            if (propertyName != null)
                RaisePropertyChanged(propertyName, value);
        }

        private Boolean VerifyCanChangeValue<T>(T oldValue,
                                                T newValue,
                                                Func<T, T, Boolean>? handleValueChanging,
                                                String propertyName)
        {
            if (Equals(oldValue, newValue))
                return false;

            if (handleValueChanging is { } validHandler &&
                !validHandler(oldValue, newValue))
                return false;

            if (!(PropertyChanging is { } valid))
                return true;

            var listeners = valid.GetInvocationList();
            foreach (var listener in listeners.OfType<Func<Object, String, Object, Object, Boolean>>())
                if (!listener(this, propertyName, oldValue!, newValue!))
                    return false;

            return true;
        }
    }
}
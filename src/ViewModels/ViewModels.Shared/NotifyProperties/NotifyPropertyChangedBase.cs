using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Das.ViewModels;

// ReSharper disable UnusedMember.Global
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace Das.Views.Mvvm
{
    public abstract class NotifyPropertyChangedBase : IViewModel
    {
        public virtual event PropertyChangedEventHandler? PropertyChanged;

        public virtual void Dispose()
        {
            IsDisposed = true;

            PropertyChanged = null;
            PropertyChanging = null;
            PropertyValueChanged = null;
        }


        private Boolean _isDisposed;

        public Boolean IsDisposed
        {
            get => _isDisposed;
            protected set => SetValue(ref _isDisposed, value);
        }
        

        /// <summary>
        ///     Allows cancellation of a property change
        /// </summary>
        public event OnPropertyChanging? PropertyChanging;

        public event Action<String, Object?>? PropertyValueChanged;

        [DebuggerStepThrough]
        [DebuggerHidden]
        [DebuggerNonUserCode]
        protected virtual void RaisePropertyChanged(String propertyName,
                                                    Object? newValue)
        {
            if (PropertyValueChanged is { } propValChanged)
                propValChanged(propertyName, newValue);

            RaisePropertyChanged(propertyName);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        [DebuggerNonUserCode]
        protected virtual void RaisePropertyChanged(String propertyName)
        {
            var handler = PropertyChanged;
            if (handler == null)
                return;

            var args = new PropertyChangedEventArgs(propertyName);
            handler(this, args);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        [DebuggerNonUserCode]
        protected virtual void RaisePropertyChanged(Object? obj,
                                                    PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(obj, args);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        [DebuggerNonUserCode]
        protected void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        [DebuggerNonUserCode]
        protected virtual Boolean SetValue<T>(ref T field,
                                              T value,
                                              [CallerMemberName] String propertyName = "")
        {
            if (!VerifyCanChangeValue(field, value, null, propertyName))
                return false;

            SetValueImpl(ref field, value, null, propertyName);
            return true;
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        [DebuggerNonUserCode]
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
        [DebuggerNonUserCode]
        protected virtual void SetValue<T>(ref T field,
                                           T newValue,
                                           AllowPropertyChangeDelegate<T> onValueChanging,
                                           [CallerMemberName] String propertyName = "")
        {
            if (!VerifyCanChangeValue(field, newValue, onValueChanging, propertyName))
                return;

            SetValueImpl(ref field, newValue, null, propertyName);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        [DebuggerNonUserCode]
        protected virtual void SetValue<T>(ref T field,
                                           T newValue,
                                           AllowPropertyChangeDelegate<T> onValueChanging,
                                           Action<T> handleValueChanged,
                                           [CallerMemberName] String propertyName = "")
        {
            if (!VerifyCanChangeValue(field, newValue, onValueChanging!, propertyName))
                return;

            SetValueImpl(ref field, newValue, handleValueChanged, propertyName);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        [DebuggerNonUserCode]
        protected virtual void SetValue<T>(ref T field,
                                           T newValue,
                                           AllowPropertyChangeDelegate<T> onValueChanging,
                                           Action handleValueChanged,
                                           [CallerMemberName] String propertyName = "")
        {
            if (!VerifyCanChangeValue(field, newValue, onValueChanging!, propertyName))
                return;

            SetValueImpl(ref field, newValue, _ => handleValueChanged(), propertyName);
        }


        [DebuggerStepThrough]
        [DebuggerHidden]
        [DebuggerNonUserCode]
        protected virtual void SetValue<T>(ref T field,
                                           T newValue,
                                           AllowPropertyChangeDelegate<T> onValueChanging,
                                           Action<T, T> handleValueChanged,
                                           [CallerMemberName] String propertyName = "")
        {
            if (!VerifyCanChangeValue(field, newValue, onValueChanging, propertyName))
                return;

            SetValueImpl(ref field, newValue, handleValueChanged, propertyName);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        [DebuggerNonUserCode]
        protected virtual void SetValue<T>(ref T? field,
                                           T newValue,
                                           InterceptPropertyChangeDelegate<T> interceptValueChanging,
                                           Action<T?> handleValueChanged,
                                           [CallerMemberName] String propertyName = "")
        where T : class
        {
            var newerValue = interceptValueChanging(field, newValue);

            if (!VerifyCanChangeValue(field, newerValue, null, propertyName))
                return;


            SetValueImpl(ref field, newerValue, handleValueChanged, propertyName);
        }


        [DebuggerStepThrough]
        [DebuggerHidden]
        [DebuggerNonUserCode]
        protected virtual void SetValue<T>(ref T field,
                                           T newValue,
                                           AllowPropertyChangeDelegate<T> onValueChanging,
                                           Func<T, Task> handleValueChangedAsync,
                                           [CallerMemberName] String propertyName = "")
        {
            if (!VerifyCanChangeValue(field, newValue, onValueChanging, propertyName))
                return;

            SetValueImpl(ref field, newValue, handleValueChangedAsync, propertyName);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        [DebuggerNonUserCode]
        protected virtual Boolean SetValue<T>(ref T field,
                                              T newValue,
                                              Action<T> onValueChanged,
                                              [CallerMemberName] String propertyName = "")
        {
            if (!VerifyCanChangeValue(field, newValue, null, propertyName))
                return false;

            SetValueImpl(ref field, newValue, onValueChanged, propertyName);
            return true;
        }

        // ReSharper disable once UnusedMember.Global
        protected Boolean TryGetPropertyHandler(out PropertyChangedEventHandler handler)
        {
            handler = PropertyChanged!;
            return handler != null!;
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        [DebuggerNonUserCode]
        // ReSharper disable once RedundantAssignment
        private void SetValueImpl<T>(ref T field,
                                     T value,
                                     Func<T, Task>? handleValueChanged,
                                     String propertyName)
        {
            field = value;

            if (handleValueChanged is { } validHandler)
                validHandler(value);

            RaisePropertyChanged(propertyName, value);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        [DebuggerNonUserCode]
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

        [DebuggerStepThrough]
        [DebuggerHidden]
        [DebuggerNonUserCode]
        private void SetValueImpl<T>(ref T field,
                                     T value,
                                     Action<T, T>? handleValueChanged,
                                     [CallerMemberName] String? propertyName = null)
        {
            var was = field;
            field = value;

            if (handleValueChanged is { } validHandler)
                validHandler(was, value);

            if (propertyName != null)
                RaisePropertyChanged(propertyName, value);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        [DebuggerNonUserCode]
        private Boolean VerifyCanChangeValue<T>(T oldValue,
                                                T newValue,
                                                AllowPropertyChangeDelegate<T>? handleValueChanging,
                                                String propertyName)
        {
            if (Equals(oldValue, newValue))
                return false;

            if (handleValueChanging is { } simpleHandler &&
                !simpleHandler(oldValue, newValue))
            {
                return false;
            }

            if (!(PropertyChanging is { } detailedHandler))
                return true;

            var listeners = detailedHandler.GetInvocationList();
            foreach (var listener in listeners.OfType<OnPropertyChanging>())
                if (!listener(this, propertyName, oldValue, newValue))
                    return false;

            return true;
        }
    }
}

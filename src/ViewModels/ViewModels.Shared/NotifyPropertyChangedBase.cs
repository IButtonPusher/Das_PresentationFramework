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
        //public event Func<Object, String, Object, Object, Boolean>? PropertyChanging;
        public event OnPropertyChanging? PropertyChanging;

        public event Action<String, Object?>? PropertyValueChanged;

        [DebuggerStepThrough]
        [DebuggerHidden]
        protected virtual void RaisePropertyChanged(String propertyName,
                                                    Object? newValue)
        {
            if (PropertyValueChanged is { } propValChanged)
                propValChanged(propertyName, newValue);

            RaisePropertyChanged(propertyName);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
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
        protected void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
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

            SetValueImpl(ref field, newValue, handleValueChanged, propertyName);
        }


        [DebuggerStepThrough]
        [DebuggerHidden]
        protected virtual void SetValue<T>(ref T field,
                                           T newValue,
                                           Func<T, T, Boolean> onValueChanging,
                                           Action<T, T> handleValueChanged,
                                           [CallerMemberName] String propertyName = "")
        {
           if (!VerifyCanChangeValue(field, newValue, onValueChanging, propertyName))
              return;

           SetValueImpl(ref field, newValue, handleValueChanged, propertyName);
        }
        
        [DebuggerStepThrough]
        [DebuggerHidden]
        protected virtual void SetValue<T>(ref T field,
                                           T newValue,
                                           Func<T, T, T> interceptValueChanging,
                                           Action<T> handleValueChanged,
                                           [CallerMemberName] String propertyName = "")
        {
            var newerValue = interceptValueChanging(field, newValue);
            
            if (!VerifyCanChangeValue(field, newerValue, null, propertyName))
                return;
            
            

            SetValueImpl(ref field, newerValue, handleValueChanged, propertyName);
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
                                     String propertyName)
        {
            field = value;

            if (handleValueChanged is { } validHandler)
                validHandler(value);

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

        [DebuggerStepThrough]
        [DebuggerHidden]
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
        private Boolean VerifyCanChangeValue<T>(T oldValue,
                                                T newValue,
                                                Func<T, T, Boolean>? handleValueChanging,
                                                String propertyName)
        {
            if (Equals(oldValue, newValue))
                return false;

            if (handleValueChanging is { } simpleHandler &&
                !simpleHandler(oldValue, newValue))
                return false;

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
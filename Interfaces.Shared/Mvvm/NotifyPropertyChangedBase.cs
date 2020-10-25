using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Das.Views.Mvvm
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged, 
                                                      IDisposable
    {
        public virtual event PropertyChangedEventHandler? PropertyChanged;

        public virtual void Dispose()
        {
            PropertyChanged = null;
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

        protected virtual Boolean SetValue<T>(ref T field, T value,
                                              [CallerMemberName] String? propertyName = null)
        {
            if (Equals(field, value))
                return false;

            field = value;

            if (propertyName != null)
                RaisePropertyChanged(propertyName);
            return true;
        }

        protected virtual Boolean SetValue<T>(ref T field,
                                              T value,
                                              Action<T> onValueChanged,
                                              [CallerMemberName] String? propertyName = null)
        {
            if (!SetValue(ref field, value, propertyName))
                return false;
            onValueChanged(value);
            return true;
        }

        // ReSharper disable once UnusedMember.Global
        protected Boolean TryGetPropertyHandler(out PropertyChangedEventHandler handler)
        {
            handler = PropertyChanged!;
            return handler != null!;
        }
    }
}

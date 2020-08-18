using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Das.ViewsModels;

namespace Das.ViewModels
{
    public abstract class BaseViewModel : IViewModel
    {
        public BaseViewModel()
        {
     
        }

        protected virtual Boolean SetValue<T>(ref T field, T value, 
            [CallerMemberName]String propertyName = null)
        {
            if (Equals(field, value))
                return false;
            
            field = value;

            RaisePropertyChanged(propertyName);
            return true;
        }

        protected virtual void RaisePropertyChanged(String propertyName)
        {
            var handler = PropertyChanged;
            if (handler == null)
                return;

            var args = new PropertyChangedEventArgs(propertyName);
            handler(this, args);
        }

        protected void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        protected Boolean TryGetPropertyHandler(out PropertyChangedEventHandler handler)
        {
            handler = PropertyChanged!;
            return handler != null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void Dispose()
        {
            PropertyChanged = null;
        }
    }
}

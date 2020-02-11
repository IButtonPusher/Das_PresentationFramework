using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Das.ViewsModels;

namespace Das.ViewModels
{
    public abstract class BaseViewModel : IViewModel
    {
        protected virtual void SetPropertyValue<T>(ref T field, T value, 
            [CallerMemberName]String propertyName = null)
        {
            if (Equals(field, value))
                return;
            
            field = value;

            RaisePropertyChanged(propertyName);
        }

        protected virtual void SetPropertyValue<TField, TValue>(ref TField field, TValue value,
            [CallerMemberName]String propertyName = null) where TValue : TField, IEquatable<TField>
        {
            if (Equals(field, value))
                return;

            field = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void RaisePropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual event PropertyChangedEventHandler PropertyChanged;
    }
}

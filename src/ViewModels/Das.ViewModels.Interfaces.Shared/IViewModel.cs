using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Das.Views.Mvvm
{
    public interface IViewModel : INotifyPropertyChanged,
                                  IDisposable
    {
        public Boolean IsDisposed { get; }

        /// <summary>
        ///     Sender, Property Name, Old Value, New Value, allow change
        /// </summary>
        event OnPropertyChanging? PropertyChanging;

        event Action<String, Object?>? PropertyValueChanged;
    }

    public delegate Boolean OnPropertyChanging(IViewModel vm,
                                               String propertyName,
                                               Object? oldValue,
                                               Object? newValue);
}

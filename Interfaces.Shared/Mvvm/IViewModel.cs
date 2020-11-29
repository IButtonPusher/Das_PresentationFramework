using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Das.Views.Mvvm
{
    public interface IViewModel : INotifyPropertyChanged, 
                                  IDisposable
    {
        /// <summary>
        ///     Sender, Property Name, Old Value, New Value, allow change
        /// </summary>
        event Func<Object, String, Object, Object, Boolean>? PropertyChanging;

        event Action<String, Object?>? PropertyValueChanged;
    }
}
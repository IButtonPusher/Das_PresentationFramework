using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Das.ViewModels
{
    public interface IObservableCommand<in TParam> : IObservableCommand
    {
        Task ExecuteAsync(TParam paramValue);

        Task ExecuteAsync(TParam[] paramValues);
    }


    public interface IObservableCommand : INotifyPropertyChanged, IEquatable<IObservableCommand>
    {
        String? Description { get; set; }

        Boolean IsExecutable { get; set; }

        Task ExecuteAsync();

        Task ExecuteAsync(Object paramValue);
    }
}
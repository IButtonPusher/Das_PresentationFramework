using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Das.ViewsModels
{
    public interface IObservableCommand<in TParam> : IObservableCommand
    {
        Task ExecuteAsync(TParam paramValue);

        Task ExecuteAsync(TParam[] paramValues);
    }


    public interface IObservableCommand : INotifyPropertyChanged, IEquatable<IObservableCommand>
    {
        Boolean IsExecutable { get; set; }

        String? Description { get; set; }

        Task ExecuteAsync();

        Task ExecuteAsync(Object paramValue);
    }
}
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Das.ViewsModels
{
    public interface IObservableCommand<in TParam> : IObservableCommand
    {
        Task Execute(TParam paramValue);

        Task Execute(TParam[] paramValues);
    }


    public interface IObservableCommand : INotifyPropertyChanged, IEquatable<IObservableCommand>
    {
        Boolean IsExecutable { get; set; }

        String Description { get; set; }

        Task Execute();

        Task Execute(Object paramValue);
    }
}
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Das.ViewsModels
{
    public interface IObservableCommand<in TParam> : IObservableCommand
    {
        Task Execute(TParam paramValue);
    }


    public interface IObservableCommand : INotifyPropertyChanged
    {
        Boolean CanExecute { get; set; }

        Task Execute();

        Task Execute(Object paramValue);
    }
}

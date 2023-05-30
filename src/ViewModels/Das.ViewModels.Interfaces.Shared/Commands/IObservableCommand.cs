using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Das.ViewModels;

public interface IObservableCommand : INotifyPropertyChanged,
                                      IEquatable<IObservableCommand>,
                                      IDisposable
{
   Task ExecuteAsync();

   Task ExecuteAsync(Object paramValue);

   String? Description { get; set; }

   Boolean IsExecutable { get; set; }
}
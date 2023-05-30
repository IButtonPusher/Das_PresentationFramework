using System;
using System.Threading.Tasks;

namespace Das.ViewModels;

public interface IObservableCommand<in TParam> :
   IObservableCommand
{
   Task ExecuteAsync(TParam paramValue);

   Task ExecuteAsync(TParam[] paramValues);
}
using System;
using System.Threading.Tasks;
using Das.Views;

namespace Das.ViewModels.Commands;

public class ObservableActionCommand<T> : ObservableCommandBase,
                                          IObservableCommand<T>
{
   public ObservableActionCommand(Action<T> action,
                                  ISingleThreadedInvoker staInvoker) : base(staInvoker)
   {
      _action = action;
   }

   public override void Execute(Object parameter)
   {
      _action(GetParamValue<T>(parameter));

      //switch (parameter)
      //{
      //    case T good:
      //        _action(good);
      //        break;

      //    case null:
      //        _action(default!);
      //        break;

      //    default:
      //        ThrowParamException<T, Object>();
      //        break;
      //}

      //if (parameter is T good)
      //    _action(good);
      //else ThrowParamException<T, Object>();
   }

   public override Task ExecuteAsync()
   {
      return ThrowParamException<T, Task>();
   }

   public override Task ExecuteAsync(Object paramValue)
   {
      _action(GetParamValue<T>(paramValue));

      //switch (paramValue)
      //{
      //    case T good:
      //        _action(good);
      //        break;

      //    case null:
      //        _action(default!);
      //        break;

      //    default:
      //        ThrowParamException<T, Object>();
      //        break;
      //}
      //if (paramValue is T good)
      //    _action(good);
      //else ThrowParamException<T, Object>();
      return Task.CompletedTask;
   }

   private readonly Action<T> _action;

   public Task ExecuteAsync(T paramValue)
   {
      _action(paramValue);
      return Task.CompletedTask;
   }

   public Task ExecuteAsync(T[] paramValues)
   {
      foreach (var paramValue in paramValues)
         _action(paramValue);
      return Task.CompletedTask;
   }
}
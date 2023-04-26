using System;
using Das.ViewModels;
using Das.Views.Controls;

namespace Das.Views.Primitives;

public interface IButtonBase<T> : IButtonBase
{
   new IObservableCommand<T>? Command { get; }
        
   T CommandParameter { get; }
}
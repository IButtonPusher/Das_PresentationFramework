using System;
using System.Collections.Generic;
using System.Text;
using Das.ViewModels;
using Das.Views.Controls;

namespace Das.Views.Primitives
{
    public interface IButtonBase<T> : IButtonBase
    {
        new IObservableCommand<T>? Command { get; }
        
        T CommandParameter { get; }
    }
}

using System;
using System.Threading.Tasks;
using Das.Views.Mvvm;

namespace Das.ViewModels;

public interface IModalVm<out T> : IModalVm,
                                   IViewModel
{
    T? DialogResult { get; }
}

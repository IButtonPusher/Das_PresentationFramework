using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Views.Mvvm;

namespace Das.ViewModels;

public interface IMutableVm : IViewModel,
                              IChangeTracking
{
}
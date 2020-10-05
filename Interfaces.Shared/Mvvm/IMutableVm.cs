using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Das.ViewModels
{
    public interface IMutableVm : IViewModel, IChangeTracking
    {
    }
}
using System;
using System.ComponentModel;

namespace Das.ViewsModels
{
    public interface IMutableVm : IViewModel, IChangeTracking
    {
    }
}

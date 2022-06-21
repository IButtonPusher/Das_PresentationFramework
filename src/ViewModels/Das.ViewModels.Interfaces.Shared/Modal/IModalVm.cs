using System;
using System.Threading.Tasks;

namespace Das.ViewModels;

public interface IModalVm : ITitledVm,
                            IModalBaseVm
{
    Boolean? DialogCompleted { get; }
}

using System;
using System.Threading.Tasks;

namespace Das.ViewModels
{
    public class BaseUiProvider
    {
        public virtual IObservableCommand<T> GetCommand<T>(Func<T, Task> action)
        {
            return new BaseObservableCommand<T>(action);
        }
    }
}

using System;
using System.Threading.Tasks;

namespace Das.Views
{
    public interface IUiProvider
    {
        void BeginInvoke(Action action);

        void Invoke(Action action);

        Task InvokeAsync(Action action);
    }
}

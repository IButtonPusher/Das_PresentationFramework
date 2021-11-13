using System;
using System.Threading.Tasks;

namespace Das.Views
{
    public interface IBootStrapper
    {
        void Run(IVisualElement view);

        IVisualBootstrapper VisualBootstrapper { get; }
    }
}
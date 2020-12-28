using System;
using System.Threading.Tasks;
using Das.Views.Panels;

namespace Das.Views
{
    public interface IBootStrapper
    {
        void Run(IView view);


        IVisualBootstrapper VisualBootstrapper { get; }
    }
}
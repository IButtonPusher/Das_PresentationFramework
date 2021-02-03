using System;
using System.Threading.Tasks;

namespace Das.Views.Panels
{
    public class StackPanel : BaseSequentialPanel
    {
        public StackPanel(IVisualBootstrapper visualBootstrapper)
        : base(visualBootstrapper)
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            _children.Dispose();
        }
    }
}
using System.Collections.Generic;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public class WrapPanel<T> : BaseSequentialPanel<T>
    {
        public WrapPanel() : base(default!,
            new SequentialRenderer(true))
        {
            
        }

        protected override IEnumerable<IVisualElement> GetChildrenToRender() => Children;

        public override void Dispose()
        {
            
        }
    }
}

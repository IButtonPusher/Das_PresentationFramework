using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public class WrapPanel<T> : BaseSequentialPanel<T>
    {
        public WrapPanel() : base(default!,
            new SequentialRenderer(true))
        {
        }

        //public override void Dispose()
        //{
        //}

        protected override IList<IVisualElement> GetChildrenToRender()
        {
            return Children;
        }
    }
}
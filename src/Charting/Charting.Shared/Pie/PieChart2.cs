using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Views.Charting
{
    public class PieChart2<TKey, TValue> : VisualElement
        where TValue : IConvertible
    {
        public PieChart2(IVisualBootstrapper visualBootstrapper) : 
            base(visualBootstrapper)
        {
        }
    }
}

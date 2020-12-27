using System;

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

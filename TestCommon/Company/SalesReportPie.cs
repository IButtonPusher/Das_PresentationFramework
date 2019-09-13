using System;
using System.Collections.Generic;
using Das.Views.Charting;
using Das.Views.Core.Drawing;

namespace TestCommon.Company
{
    public class SalesReportPie : IPieData<String, Double>
    {
        public SalesReportPie()
        {
            var items = new List<ProductSold>();

            items.Add(new ProductSold("widgets", 11000));
            items.Add(new ProductSold("warantees", 11000));

            Items = items;

            var colors = new Dictionary<String, IBrush>();
            colors.Add("widgets", new Brush(new Color(255, 0, 255)));
            colors.Add("warantees", new Brush(new Color(255, 255, 20)));
            ItemColors = colors;
        }

        public IEnumerable<IDataPoint<String, Double>> Items { get; }
        public IDictionary<string, IBrush> ItemColors { get; }
    }
}

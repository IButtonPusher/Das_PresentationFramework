using System;
using Das.Views.Charting;

namespace TestCommon.Company
{
    public class ProductSold : IDataPoint<String, Double>
    {
        public ProductSold(String description, Double value)
        {
            Description = description;
            Value = value;
        }

        public String Description { get; }
        public Double Value { get; }

        public override String ToString() => Description + ": " + Value;
    }
}

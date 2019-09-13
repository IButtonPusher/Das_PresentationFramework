using System;
using Das.Views.Charting;

namespace TestCommon.Company
{
    public class ProductSold : IDataPoint<String, Double>
    {
        public ProductSold(string description, double value)
        {
            Description = description;
            Value = value;
        }

        public string Description { get; }
        public double Value { get; }

        public override string ToString() => Description + ": " + Value;
    }
}

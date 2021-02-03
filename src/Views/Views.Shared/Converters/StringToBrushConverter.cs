using System;
using System.Reflection;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;

namespace Das.Views.Converters
{
    public class StringToBrushConverter : BaseConverter<String, SolidColorBrush>
    {
        public static StringToBrushConverter Instance { get; } = new();

        public sealed override SolidColorBrush Convert(String input)
        {
            var staticProp = typeof(SolidColorBrush).GetProperty(input, BindingFlags.Static |
                                                                        BindingFlags.Public);

            if (staticProp != null)
                return (SolidColorBrush) staticProp.GetValue(null, null);

            throw new InvalidCastException();
        }


    }
}

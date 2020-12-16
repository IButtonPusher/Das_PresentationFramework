using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.DataBinding;

namespace Das.Views.Converters
{
    public class StringToBrushConverter : BaseConverter<String, SolidColorBrush>
    {
        public static StringToBrushConverter Instance { get; } = new StringToBrushConverter();

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

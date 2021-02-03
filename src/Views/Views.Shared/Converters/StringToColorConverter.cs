using System;
using System.Reflection;
using Das.Views.Core.Drawing;

namespace Das.Views.Converters
{
    public class StringToColorConverter : BaseConverter<String, Color>
    {
        public static StringToColorConverter Instance { get; } = new();

        public override Color Convert(String input)
        {
            var staticProp = typeof(Color).GetProperty(input, BindingFlags.Static |
                                                              BindingFlags.Public);

            if (staticProp != null)
                return (Color) staticProp.GetValue(null, null);

            throw new InvalidCastException();
        }
    }
}

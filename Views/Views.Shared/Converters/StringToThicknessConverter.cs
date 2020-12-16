using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Converters
{
    public class StringToThicknessConverter : BaseConverter<String, Thickness>
    {
        public static StringToThicknessConverter Instance { get; } = new StringToThicknessConverter();
        
        
        public sealed override Thickness Convert(String input)
        {
            var tokens = input.Split(_splitters, StringSplitOptions.RemoveEmptyEntries);

            switch (tokens.Length)
            {
                case 1:
                    return new Thickness(Double.Parse(tokens[0]));
                
                case 2:
                    return new Thickness(Double.Parse(tokens[0]),
                        Double.Parse(tokens[1]));
                
                case 4:
                    return new Thickness(Double.Parse(tokens[0]),
                        Double.Parse(tokens[1]),
                        Double.Parse(tokens[2]),
                        Double.Parse(tokens[3]));
                
                default:
                    throw new InvalidCastException();

            }
        }

       

        private static readonly Char[] _splitters = new Char[] {' ', ','};
    }
}

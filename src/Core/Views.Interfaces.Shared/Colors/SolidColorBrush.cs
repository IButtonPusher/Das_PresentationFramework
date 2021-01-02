using System;
using System.Threading.Tasks;
using Das.Extensions;
// ReSharper disable UnusedMember.Global

namespace Das.Views.Core.Drawing
{
    public class SolidColorBrush : IEquatable<SolidColorBrush>, 
                                   IBrush
    {
        public SolidColorBrush(Color color)
        {
            Color = color;
            Opacity = 1;
        }

        public SolidColorBrush(Color color,
                               Double opacity)
        {
            Color = Color.FromArgb(Convert.ToByte(opacity * 255), color.R, color.G, color.B);
            Opacity = opacity;
        }

        public SolidColorBrush(Byte red,
                               Byte green,
                               Byte blue,
                               Double alpha)
        {
            Color = Color.FromArgb(Convert.ToByte(alpha * 255), red, green, blue);
            Opacity = alpha;
        }
        
        public SolidColorBrush(Byte red,
                               Byte green,
                               Byte blue)
        {
            Color = Color.FromRgb(red, green, blue);
            Opacity = 1;
        }
        

        public Boolean Equals(IBrush other)
        {
            return other is SolidColorBrush scb && Equals(scb);
        }

        //IColor IBrush.Color => Color;

        public Boolean Equals(SolidColorBrush? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Color.Equals(other.Color);
        }

        public static SolidColorBrush Black { get; } = new SolidColorBrush(Color.Black);

        public Color Color { get; }

        public static SolidColorBrush DarkGray => _darkGray.Value;
        
        public static SolidColorBrush Green => _green.Value;

        public static SolidColorBrush Tranparent => _transparent.Value;

        public virtual Boolean IsInvisible => Color.A == 0;

        public Double Opacity { get;  }

        public IBrush GetWithOpacity(Double opacity)
        {
            return new SolidColorBrush(Color, opacity);
        }

        public static SolidColorBrush LightGray => _lightGray.Value;

        public static SolidColorBrush Purple => _purple.Value;

        public static SolidColorBrush Pink => _pink.Value;
        
        public static SolidColorBrush Orange => _orange.Value;


        public static SolidColorBrush Red => _red.Value;

        public static SolidColorBrush White { get; } = new SolidColorBrush(Color.White);

        public override Boolean Equals(Object obj)
        {
            if (obj is SolidColorBrush b)
                return Equals(b);

            return false;
        }

        public static SolidColorBrush FromRgb(Byte r, 
                                              Byte g, 
                                              Byte b)
        {
            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        public static SolidColorBrush FromArgb(Byte alpha,
                                               Byte r, 
                                              Byte g, 
                                              Byte b)
        {
            return new SolidColorBrush(Color.FromArgb(alpha, r, g, b));
        }

        public override Int32 GetHashCode()
        {
            return Color != null ? Color.GetHashCode() : 0;
        }

        public override String ToString()
        {
            return "Brush: " + Color + (Opacity.AreEqualEnough(1.0) ? String.Empty : " " + Opacity);
        }

        private static readonly Lazy<SolidColorBrush> _darkGray = new Lazy<SolidColorBrush>(()
            => new SolidColorBrush(Color.DarkGray));
        
        private static readonly Lazy<SolidColorBrush> _orange = new Lazy<SolidColorBrush>(()
            => new SolidColorBrush(Color.Orange));
        
        private static readonly Lazy<SolidColorBrush> _green = new Lazy<SolidColorBrush>(()
            => new SolidColorBrush(Color.Green));

        private static readonly Lazy<SolidColorBrush> _transparent = new Lazy<SolidColorBrush>(()
            => new SolidColorBrush(Color.Transparent));

        private static readonly Lazy<SolidColorBrush> _lightGray = new Lazy<SolidColorBrush>(()
            => new SolidColorBrush(Color.LightGray));

        private static readonly Lazy<SolidColorBrush> _red = new Lazy<SolidColorBrush>(()
            => new SolidColorBrush(Color.Red));

        private static readonly Lazy<SolidColorBrush> _purple = new Lazy<SolidColorBrush>(()
            => new SolidColorBrush(Color.Purple));

        private static readonly Lazy<SolidColorBrush> _pink = new Lazy<SolidColorBrush>(()
            => new SolidColorBrush(Color.Pink));
    }
}
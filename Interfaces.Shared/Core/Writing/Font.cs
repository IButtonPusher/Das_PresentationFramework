using System;

namespace Das.Views.Core.Writing
{
    public class Font : IEquatable<Font>, IFont, IEquatable<IFont>
    {
        public Font(double size, string familyName, FontStyle fontStyle)
        {
            Size = size;
            FamilyName = familyName;
            FontStyle = fontStyle;

            var familyHash = String.Intern(FamilyName).GetHashCode();

            _hashCode = Convert.ToInt32(Size) +
                        ((Int32)FontStyle << 8) +
                        (familyHash << 11);
        }

        private readonly Int32 _hashCode;

        public Double Size { get; }

        public String FamilyName { get; }

        public FontStyle FontStyle { get; }

        public bool Equals(Font other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (!other.Size.AreEqualEnough(Size))
                return false;

            if (other.FontStyle != FontStyle)
                return false;

            return other.FamilyName.Equals(FamilyName, StringComparison.Ordinal);
        }

        public override bool Equals(object ooooo)
        {
            switch (ooooo)
            {
                case IFont fu:
                    return Equals(fu);
            }

            return false;
        }

        public bool Equals(IFont other)
        {

            if (ReferenceEquals(other, null))
                return false;

            if (!other.Size.AreEqualEnough(Size))
                return false;

            if (other.FontStyle != FontStyle)
                return false;

            return other.FamilyName.Equals(FamilyName, StringComparison.Ordinal);
        }

        public override int GetHashCode() => _hashCode;

        public static Font operator *(Font font, Double val)
        {
            if (val.AreEqualEnough(1))
                return font;

            if (font == null)
                return null;

            return new Font(font.Size * val, font.FamilyName, font.FontStyle);
        }

        public override string ToString() => $"{FamilyName} {FontStyle}: {Size}";
    }
}
namespace Das.Views.Core.Geometry
{
    public class RoundedSize : IRoundedSize
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public bool Equals(IRoundedSize other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Width == other.Width &&
                   Height == other.Height;
        }

        public override bool Equals(object obj)
            => (obj is IRoundedSize isize && Equals(isize));

        public override int GetHashCode()
        {
            unchecked
            {
                return (Width * 397) ^ Height;
            }
        }
    }
}

namespace Das.Views.Extended.Core
{
    public class Vector2
    {
        public float X { get; set; }
        /// <summary>The Y component of the vector.</summary>
        public float Y { get; set; }

    public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2() { }
    }
}

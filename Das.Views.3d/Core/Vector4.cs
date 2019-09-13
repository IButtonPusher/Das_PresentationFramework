namespace Das.Views.Extended.Core
{
    public class Vector4 : Vector3
    {
        public float W;

        public Vector4()
        {
            
        }

        public Vector4(float x, float y, float z, float w) : base(x, y, z)
        {
            W = w;
        }
    }
}

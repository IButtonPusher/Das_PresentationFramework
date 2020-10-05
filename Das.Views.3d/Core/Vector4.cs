using System;
using System.Threading.Tasks;

namespace Das.Views.Extended
{
    public class Vector4 : Vector3
    {
        public Vector4()
        {
        }

        public Vector4(Single x, Single y, Single z, Single w) : base(x, y, z)
        {
            W = w;
        }

        public Single W;
    }
}
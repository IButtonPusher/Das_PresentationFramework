using System;
using System.Threading.Tasks;

namespace Das.Views.Transforms
{
    public class IdentityTransform : ITransform
    {
        private IdentityTransform()
        {
        }

        public Boolean IsIdentity => true;

        public TransformationMatrix Value => TransformationMatrix.Identity;

        public static readonly IdentityTransform Instance = new IdentityTransform();
    }
}
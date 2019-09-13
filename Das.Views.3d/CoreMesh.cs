using System;
using Das.Views.Extended.Core;

namespace Das.Views.Extended
{
    public class CoreMesh : IVisual3dElement//VisualElement, IVisual3dElement
    {
        public IPoint3d Position { get; protected set; }
        public IPoint3d Rotation { get; protected set; }
        public IPoint3d[] Vertices { get; protected set; }

        public Face[] Faces { get; protected set; }
        public void Rotate(float x, float y, float z)
        {
            Rotation = Rotation.Rotate(x, y, z);
        }
    }
}

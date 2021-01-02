using System;
using System.Collections.Generic;
using System.Linq;

namespace Das.Views.Extended
{
    public class Core3dModel : IVisual3dElement
    {
        public Core3dModel(Vector3 position, 
                           Vector3 rotation, 
                           IEnumerable<CoreMesh> meshes)
        {
            Position = position;
            Rotation = rotation;
            Meshes = meshes.ToArray();
        }

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public CoreMesh[] Meshes { get; }

        IPoint3D I3DElement.Position => Position;

        IPoint3D I3DElement.Rotation => Rotation;

        void I3DElement.Rotate(Single x, Single y, Single z)
        {
            throw new NotImplementedException();
        }

        // ReSharper disable once CoVariantArrayConversion
        IMesh[] IVisual3dElement.Meshes => Meshes;
    }
}

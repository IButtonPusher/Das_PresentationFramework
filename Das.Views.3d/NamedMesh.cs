using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Extended
{
    public class NamedMesh : CoreMesh
    {
        public NamedMesh(IEnumerable<IPoint3D> vertices, 
                         IEnumerable<Face> faces, String name) : base(vertices, faces)
        {
            Name = name;
        }

        public String Name { get; }
    }
}

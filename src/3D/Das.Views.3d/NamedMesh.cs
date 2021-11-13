using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Extended
{
   public class NamedMesh : CoreMesh
   {
      public NamedMesh(IEnumerable<IPoint3D> vertices,
                       IEnumerable<Face> faces,
                       String name) : base(vertices, faces)
      {
         Name = name;
      }

      public override String ToString()
      {
         return Name + " " + base.ToString();
      }

      public String Name { get; }
   }
}

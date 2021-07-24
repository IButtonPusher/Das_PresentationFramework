using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Extended.Core;

namespace Das.Views.Extended
{
   /// <summary>
   ///    A collection of three-dimensional points (vertices) and faces that
   ///    are relative to a common position and rotation
   /// </summary>
   public class CoreMesh : IMesh
   {
      public CoreMesh()
      {
         Position = new Vector3();
         Rotation = new Vector3();
         Vertices = new IPoint3D[0];
         Faces = new Face[0];
      }

      public CoreMesh(IEnumerable<IPoint3D> vertices) : this()
      {
         Vertices = vertices.ToArray();
      }

      public CoreMesh(IEnumerable<IPoint3D> vertices,
                      IEnumerable<Face> faces) : this(vertices)
      {
         Faces = faces.ToArray();
      }

      public IPoint3D Position { get; protected set; }

      public IPoint3D Rotation { get; protected set; }

      public IPoint3D[] Vertices { get; protected set; }

      public Face[] Faces { get; protected set; }

      public void Rotate(Single x,
                         Single y,
                         Single z)
      {
         Rotation = Rotation.Rotate(x, y, z);
      }

      public void Transform(Transformation3D xform)
      {
         Rotate((Single) xform.Rotation.X, (Single) xform.Rotation.Y, (Single) xform.Rotation.Z);

         //Position = new Vector3(Position.X + xform.PositionOffset.X,
         //    Position.Y + xform.PositionOffset.Y,
         //    Position.Z + xform.PositionOffset.Z);

         for (var c = 0; c < Vertices.Length; c++)
         {
            var v = Vertices[c];
            Vertices[c] = new Vector3(v.X * xform.Scale.X,
               v.Y * xform.Scale.Y,
               v.Z * xform.Scale.Z);
         }
      }
   }
}

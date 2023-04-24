using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Extended.Core;

namespace Das.Views.Extended;

/// <summary>
///    A collection of three-dimensional points (vertices) and faces that
///    are relative to a common position and rotation
/// </summary>
public class CoreMesh : IMesh
{
   public CoreMesh()
   {
      //Position = new Vector3();
      //Rotation = new Vector3();
      Vertices = Array.Empty<IPoint3D>();
      Faces = Array.Empty<Face>();

      Transformation = Transformation3D.Identity;
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

   public IPoint3D Position => Transformation.PositionOffset;

   public IPoint3D Rotation => Transformation.Rotation;  //{ get; protected set; }

   public IPoint3D[] Vertices { get; protected set; }

   public Face[] Faces { get; protected set; }

   public Transformation3D Transformation { get; set; }

   public void Rotate(Single x,
                      Single y,
                      Single z)
   {
      throw new NotImplementedException();
      //Rotation = Rotation.Rotate(x, y, z);
   }

   //public void Transform(Transformation3D xform)
   //{
   //   Rotate((Single) xform.Rotation.X, (Single) xform.Rotation.Y, (Single) xform.Rotation.Z);

   //   for (var c = 0; c < Vertices.Length; c++)
   //   {
   //      var v = Vertices[c];
   //      Vertices[c] = new Vector3(v.X * xform.Scale.X,
   //         v.Y * xform.Scale.Y,
   //         v.Z * xform.Scale.Z);
   //   }
   //}

   public override String ToString()
   {
      return "Vertices: " + Vertices.Length + " pos: " + Position +
             " rotation: " + Rotation;
   }
}
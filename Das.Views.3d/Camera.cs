using Das.Views.Rendering;
using System;
using System.Collections.Generic;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Extended.Core;
using Das.Views.Extended.Runtime;

namespace Das.Views.Extended
{
    public class Camera : BasePerspective, ICamera<LineFrame>
    {
        private readonly IScene _scene;

        public Camera(IPoint3d position, IPoint3d rotation, IPoint3d target, IScene scene)
        {
            _scene = scene;
            Position = position;
            Rotation = rotation;
            Target = target;
            FocalPoint = target;
            FieldOfView = 0.78f;
            AspectRatio = 1;
            NearZenith = 0.1f;
            FarZenith = 1;
        }

        public IPoint3d Position { get; }
        public IPoint3d Rotation { get; }
        public void Rotate(float x, float y, float z)
        {
            throw new NotImplementedException();
        }

        public IPoint3d Target { get; }
        public IPoint3d FocalPoint { get; }
        public float AspectRatio { get; private set; }
        public float FieldOfView { get; }
        public float NearZenith { get; }
        public float FarZenith { get; }
        IFrame ICamera.GetFrame(ISize targetSize) => GetFrame(targetSize);

        public LineFrame GetFrame(ISize targetSize)
        {
            AspectRatio = (float)(targetSize.Width / targetSize.Height);
            var view = Matrix.LookAtLH(this);
            var projectionJatrix = GetProjection();

            var allTriangles = new List<IMultiLine>();

            foreach (var element in _scene.VisualElements)
            {
                var j1 = Matrix.RotationYawPitchRoll(element);
                var j2 = Matrix.Translation(element.Position);

                var matrixReloaded = j1 * j2;
                var aviatrix = matrixReloaded * view * projectionJatrix;

                var newTriangles = new Triangle[element.Faces.Length];
                var triangleCounter = 0;

                foreach (var face in element.Faces)
                {
                    var vertexA = element.Vertices[face.A];
                    var vertexB = element.Vertices[face.B];
                    var vertexC = element.Vertices[face.C];

                    var pixelA = Project(targetSize, vertexA, aviatrix);
                    var pixelB = Project(targetSize, vertexB, aviatrix);
                    var pixelC = Project(targetSize, vertexC, aviatrix);
                    newTriangles[triangleCounter++] = new Triangle(pixelA, pixelB, pixelC);
                }

                allTriangles.AddRange(newTriangles);
            }
            
            return new LineFrame(allTriangles, targetSize);
        }

        private static IPoint Project(ISize availableSpace, IPoint3d vertex, Matrix aviatrix)
        {
            var point = Vector3.TransformCoordinate(vertex, aviatrix);

            var x = point.X * availableSpace.Width + availableSpace.Width / 2.0f;
            var y = -point.Y * availableSpace.Height + availableSpace.Height / 2.0f;
            return new Point(x, y);
        }

        protected Matrix GetProjection()
        {
            var zfar = FarZenith;
            var znear = NearZenith;

            var num1 = (float)(1.0 / Math.Tan(FieldOfView * 0.5));
            var num2 = num1 / AspectRatio;
            var right = znear / num2;
            var top = znear / num1;

            //////
            var left = -right;
            var bottom = -top;

            //////
            var num = zfar / (zfar - znear);
            var result = new Matrix();
            result.M11 = 2.0f * znear / (right - left);
            result.M22 = 2.0f * znear / (top - bottom);
            result.M31 = (left + right) / (left - right);
            result.M32 = (top + bottom) / (bottom - top);
            result.M33 = num;
            result.M34 = 1f;
            result.M43 = -znear * num;
            //////
            result.M31 *= -1f;
            result.M32 *= -1f;
            result.M33 *= -1f;
            result.M34 *= -1f;

            return result;
        }
    }
}

using Das.Views.Extended;
using Das.Views.Extended.Core;

namespace TestCommon
{
    public class TestCube : CoreMesh
    {
        public TestCube()
        {
            Position = new Vector3();
            Rotation = new Vector3();


            Vertices = new IPoint3d[]
            {
                  new Vector3(-1, 1, 1),
                  new Vector3(1, 1, 1),
                  new Vector3(-1, -1, 1),
                  new Vector3(1, -1, 1),
                  new Vector3(-1, 1, -1),
                  new Vector3(1, 1, -1),
                  new Vector3(1, -1, -1),
                  new Vector3(-1, -1, -1),
            };

            Faces = new[]
            {
             new Face { A = 0, B = 1, C = 2 },
             new Face { A = 1, B = 2, C = 3 },
             new Face { A = 1, B = 3, C = 6 },
             new Face { A = 1, B = 5, C = 6 },
             new Face { A = 0, B = 1, C = 4 },
             new Face { A = 1, B = 4, C = 5 },

             new Face { A = 2, B = 3, C = 7 },
             new Face { A = 3, B = 6, C = 7 },
             new Face { A = 0, B = 2, C = 7 },
             new Face { A = 0, B = 4, C = 7 },
             new Face { A = 4, B = 5, C = 6 },
             new Face { A = 4, B = 6, C = 7 },
            };
        }
    }
}

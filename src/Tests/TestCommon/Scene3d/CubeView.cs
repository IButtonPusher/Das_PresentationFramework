using System;
using Das.Views.Panels;

namespace TestCommon
{
    public sealed class CubeView : View<Object>
    {
        public CubeView()
        {
            Content = new TestCube();
        }
    }
}

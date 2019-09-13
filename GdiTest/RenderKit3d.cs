using System;
using System.Drawing;
using Das.Gdi;
using Das.Views;
using Das.Views.Extended;
using Das.Views.Extended.Core;
using Das.Views.Kits;

namespace GdiTest
{
    public class RenderKit3d : CoreRenderKit
    {
        private readonly CoreMesh _mesh;

        public RenderKit3d(IBackedByBitmap control, IView view) 
            : base(control, view, view.StyleContext,
                  new Camera(new Vector3(0, 0, 10.0f), new Vector3(),  Vector3.Zero),
                  null, null)
        {
            
            _mesh = (CoreMesh)view.Content;
        }

        protected override void Render(Graphics g)
        {
            _mesh.Rotate(0.01f, 0.01f, 0);
            base.Render(g);
        }
    }
}

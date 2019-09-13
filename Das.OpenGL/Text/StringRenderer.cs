using System;
using System.Linq;
using Das.OpenGL.Text;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;

namespace Das.OpenGL
{
    public class StringRenderer : IStringRenderer
    {
        public StringRenderer(FontProvider fontProvider, IGLContext context)
        {
            _fontProvider = fontProvider;
            _context = context;
        }

        private readonly FontProvider _fontProvider;
        private readonly IGLContext _context;

        public void DrawString(String text, IFont font, IBrush brush, IPoint point)
        {
            var fontBitmapEntry = _fontProvider.GetRenderer(font);

            var width = _context.Size.Width;
            var height = _context.Size.Height;

            var x = Convert.ToInt32(point.X);
            var y = Convert.ToInt32(point.Y);
            var r = brush.Color.R;
            var g = brush.Color.G;
            var b = brush.Color.B;
            
            GL.glMatrixMode(GL.PROJECTION);
            GL.glPushMatrix();
            GL.glLoadIdentity();

            var viewport = new int[4];
            GL.glGetIntegerv(GL.VIEWPORT, viewport);
            GL.glOrtho(0, width, height, -10, -1, 1);

            //  Create the appropriate modelview matrix.
            GL.glMatrixMode(GL.MODELVIEW);
            GL.glPushMatrix();
            GL.glLoadIdentity();
            GL.glColor3f(r, g, b);
            //GL.glColor3f(1f, 0f, 0f);
            GL.glRasterPos2i(x, y);

            GL.glPushAttrib(GL.LIST_BIT | GL.CURRENT_BIT |
                GL.ENABLE_BIT | GL.TRANSFORM_BIT);
            GL.glColor3f(r, g, b);
            
            GL.glDisable(GL.LIGHTING);
            GL.glDisable(GL.TEXTURE_2D);
            GL.glDisable(GL.DEPTH_TEST);
            GL.glRasterPos2i(x, y);

            //  Set the list base.
            GL.glListBase(fontBitmapEntry.ListBase);
            
            var lists = text.Select(c => (byte)c).ToArray();
            
            GL.glCallLists(lists.Length, GL.UNSIGNED_BYTE, lists);
            GL.glFlush();

            
            GL.glPopAttrib();
            GL.glPopMatrix();
            
            GL.glMatrixMode(GL.PROJECTION);
            GL.glPopMatrix();
            GL.glMatrixMode(GL.MODELVIEW);
        }
    }
}

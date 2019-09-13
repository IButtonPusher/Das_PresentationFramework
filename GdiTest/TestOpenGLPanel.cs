using System;
using Das.OpenGL;
using Das.OpenGL.Windows;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace GdiTest
{
    public class TestOpenGLPanel : OpenGLPanel
    {
        private readonly IFont _font;
        private readonly IPen _pen;
        private readonly float[] _zoo1PieData;
        private const Double TwoPi = 2.0 * Math.PI;

        public TestOpenGLPanel(IView view, IStyleContext styleContext)
            : base(view, styleContext)
        {
            _font = new Font(18, "Segoe UI", FontStyle.Regular);
            _pen = new Pen(Color.Black, 2);
            var rng = new Random();
            float[] z = {
                (float)rng.NextDouble(),(float)rng.NextDouble(),
                (float)rng.NextDouble(), (float)rng.NextDouble(),
                (float)rng.NextDouble(),(float)rng.NextDouble(),
                (float)rng.NextDouble(),(float)rng.NextDouble()};
            _zoo1PieData = z;
        }

        protected override void Render(IRenderContext context)
        {
            context.DrawString("hello world", _font, Brush.Black, 
                new Point(50,50));

            context.DrawLine(_pen, new Point(10,10), new Point(150,50));

            PieChart1(200, 200, 200);
        }

        private void PieChart1(int x, int y, int radius)
        {
            var pntCnt = _zoo1PieData.Length;
          
            Double startPos = 0;
            var anglePcts = new Double[pntCnt];
            
            Double valSum = 0;
            
            for (var i = 0; i < pntCnt; ++i)
                valSum += _zoo1PieData[i];
            
            for (var i = 0; i < pntCnt; ++i)
                anglePcts[i] = (_zoo1PieData[i] / valSum) * 100;

            for (var i = 0; i < pntCnt; ++i)
            {
                var c = (i+1) * (255f / (pntCnt+1)) / 255;

                GL.glBegin(GL.TRIANGLE_FAN);
                GL.glColor3f(c, 0, c);
                
                GL.glVertex3f(x, y, 0);

                var ceil = startPos + anglePcts[i];

                for (var j = startPos; j <= ceil; j += 0.2)
                    DrawSemiSlice(x, y, radius, j);
                
                DrawSemiSlice(x,y, radius, ceil);

                startPos = ceil;

                GL.glEnd();
            }
        }

        private static void DrawSemiSlice(Int32 x, Int32 y, Int32 radius, Double j)
        {
            var t = (TwoPi * j / 100.0) + Math.PI;

            var xVert = (float)(x - Math.Sin(t) * radius);
            var yVert = (float)(y + Math.Cos(t) * radius);

            GL.glVertex3f(xVert, yVert, 0);
        }
    }
}

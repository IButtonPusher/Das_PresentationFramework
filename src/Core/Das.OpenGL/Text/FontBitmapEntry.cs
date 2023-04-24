using System;
using System.Linq;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;

namespace Das.OpenGL;

public class FontBitmapEntry : IFontRenderer
{
   private readonly IGLContext _context;

   public FontBitmapEntry(IFont font, IGLContext context, UInt32 listBase, 
                          UInt32 listCount)
   {
      Font = font;
      _context = context;
      Height = (Int32)(font.Size * (16.0f / 12.0f));
      ListBase = listBase;
      ListCount = listCount;
   }

   public IntPtr HDC => _context.DeviceContextHandle;

   public IntPtr HRC => _context.RenderContextHandle;

   public String FaceName => Font.FamilyName;

   public Int32 Height { get; }

   public UInt32 ListBase { get; }

   public UInt32 ListCount { get; }

   public IFont Font { get; }

   public void DrawString<TBrush, TPoint>(String text,
                                          TBrush brush,
                                          TPoint point2D)
      where TBrush : IBrush
      where TPoint : IPoint2D
   {
      var width = _context.Size.Width;
      var height = _context.Size.Height;

      var x = Convert.ToInt32(point2D.X);
      var y = Convert.ToInt32(point2D.Y);

      if (!(brush is SolidColorBrush scb))
         throw new NotImplementedException();

      var r = scb.Color.R;
      var g = scb.Color.G;
      var b = scb.Color.B;

      GL.glMatrixMode(GL.PROJECTION);
      GL.glPushMatrix();
      GL.glLoadIdentity();

      var viewport = new Int32[4];
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
      GL.glListBase(ListBase);

      var lists = text.Select(c => (Byte)c).ToArray();

      GL.glCallLists(lists.Length, GL.UNSIGNED_BYTE, lists);
      GL.glFlush();


      GL.glPopAttrib();
      GL.glPopMatrix();

      GL.glMatrixMode(GL.PROJECTION);
      GL.glPopMatrix();
      GL.glMatrixMode(GL.MODELVIEW);
   }

   public void DrawStringInRect<TBrush, TRectangle>(String s,
                                                    TBrush brush,
                                                    TRectangle bounds)
      where TBrush : IBrush
      where TRectangle : IRectangle
   {
      DrawString(s, brush, bounds.TopLeft);
   }

   public ValueSize MeasureString(String text) => throw new NotImplementedException();

   public void Dispose()
   {
            
   }
}
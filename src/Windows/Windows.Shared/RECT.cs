using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
   //[StructLayout(LayoutKind.Sequential)]
   //public struct RECT
   //{
   //   public Int32 left;
   //   public Int32 top;
   //   public Int32 right;
   //   public Int32 bottom;

   //   public RECT(Int32 left,
   //               Int32 top,
   //               Int32 right,
   //               Int32 bottom)
   //   {
   //      this.left = left;
   //      this.top = top;
   //      this.right = right;
   //      this.bottom = bottom;
   //   }

   //   public RECT(Rectangle r)
   //   {
   //      left = Convert.ToInt32(r.Left);
   //      top = Convert.ToInt32(r.Top);
   //      right = Convert.ToInt32(r.Right);
   //      bottom = Convert.ToInt32(r.Bottom);
   //   }

   //   public static RECT FromXYWH(Int32 x,
   //                               Int32 y,
   //                               Int32 width,
   //                               Int32 height)
   //   {
   //      return new RECT(x, y, x + width, y + height);
   //   }

   //   public SIZE Size => new SIZE(right - left, bottom - top);
   //}

   /// <summary>
   /// The RECT structure defines the coordinates of the upper-left and lower-right corners of a rectangle.
   /// </summary>
   /// <see cref="http://msdn.microsoft.com/en-us/library/dd162897%28VS.85%29.aspx"/>
   /// <remarks>
   /// By convention, the right and bottom edges of the rectangle are normally considered exclusive.
   /// In other words, the pixel whose coordinates are ( right, bottom ) lies immediately outside of the the rectangle.
   /// For example, when RECT is passed to the FillRect function, the rectangle is filled up to, but not including,
   /// the right column and bottom row of pixels. This structure is identical to the RECTL structure.
   /// </remarks>
   [StructLayout(LayoutKind.Sequential)]
   public struct RectStruct
   {
      /// <summary>
      /// The x-coordinate of the upper-left corner of the rectangle.
      /// </summary>
      public Int32 Left;

      /// <summary>
      /// The y-coordinate of the upper-left corner of the rectangle.
      /// </summary>
      public Int32 Top;

      /// <summary>
      /// The x-coordinate of the lower-right corner of the rectangle.
      /// </summary>
      public Int32 Right;

      /// <summary>
      /// The y-coordinate of the lower-right corner of the rectangle.
      /// </summary>
      public Int32 Bottom;      
   }
}

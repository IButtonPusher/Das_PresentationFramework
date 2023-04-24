using System;
using System.Runtime.InteropServices;
using FT_Long = System.IntPtr;
using FT_ULong = System.UIntPtr;
// ReSharper disable All

namespace Das.OpenGL.Text;

[StructLayout(LayoutKind.Sequential)]
public struct BBox : IEquatable<BBox>
{
   #region Fields

   private FT_Long xMin, yMin;
   private FT_Long xMax, yMax;

   #endregion

   #region Constructors

   /// <summary>
   /// Initializes a new instance of the <see cref="BBox"/> struct.
   /// </summary>
   /// <param name="left">The left bound.</param>
   /// <param name="bottom">The bottom bound.</param>
   /// <param name="right">The right bound.</param>
   /// <param name="top">The upper bound.</param>
   public BBox(Int32 left, Int32 bottom, Int32 right, Int32 top)
   {
      xMin = (IntPtr)left;
      yMin = (IntPtr)bottom;
      xMax = (IntPtr)right;
      yMax = (IntPtr)top;
   }

   #endregion

   #region Properties

   /// <summary>
   /// Gets the horizontal minimum (left-most).
   /// </summary>
   public Int32 Left => (Int32)xMin;

   /// <summary>
   /// Gets the vertical minimum (bottom-most).
   /// </summary>
   public Int32 Bottom => (Int32)yMin;

   /// <summary>
   /// Gets the horizontal maximum (right-most).
   /// </summary>
   public Int32 Right => (Int32)xMax;

   /// <summary>
   /// Gets the vertical maximum (top-most).
   /// </summary>
   public Int32 Top => (Int32)yMax;

   #endregion

   #region Operators

   /// <summary>
   /// Compares two instances of <see cref="BBox"/> for equality.
   /// </summary>
   /// <param name="left">A <see cref="BBox"/>.</param>
   /// <param name="right">Another <see cref="BBox"/>.</param>
   /// <returns>A value indicating equality.</returns>
   public static Boolean operator ==(BBox left, BBox right) => left.Equals(right);

   /// <summary>
   /// Compares two instances of <see cref="BBox"/> for inequality.
   /// </summary>
   /// <param name="left">A <see cref="BBox"/>.</param>
   /// <param name="right">Another <see cref="BBox"/>.</param>
   /// <returns>A value indicating inequality.</returns>
   public static Boolean operator !=(BBox left, BBox right) => !left.Equals(right);

   #endregion

   #region Methods

   /// <summary>
   /// Compares this instance of <see cref="BBox"/> to another for equality.
   /// </summary>
   /// <param name="other">A <see cref="BBox"/>.</param>
   /// <returns>A value indicating equality.</returns>
   public Boolean Equals(BBox other) =>
      xMin == other.xMin &&
      yMin == other.yMin &&
      xMax == other.xMax &&
      yMax == other.yMax;

   /// <summary>
   /// Compares this instance of <see cref="BBox"/> to an object for equality.
   /// </summary>
   /// <param name="obj">An object.</param>
   /// <returns>A value indicating equality.</returns>
   public override Boolean Equals(Object obj)
   {
      if (obj is BBox)
         return this.Equals((BBox)obj);

      return false;
   }

   /// <summary>
   /// Gets a unique hash code for this instance.
   /// </summary>
   /// <returns>A hash code.</returns>
   public override Int32 GetHashCode() => xMin.GetHashCode() ^ yMin.GetHashCode() ^ xMax.GetHashCode() ^ yMax.GetHashCode();

   /// <summary>
   /// Gets a string that represents this instance.
   /// </summary>
   /// <returns>A string representation of this instance.</returns>
   public override String ToString() => "Min: (" + (Int32)xMin + ", " + (Int32)yMin + "), Max: (" + (Int32)xMax + ", " + (Int32)yMax + ")";

   #endregion
}
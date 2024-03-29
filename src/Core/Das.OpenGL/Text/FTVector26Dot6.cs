﻿using System;
using System.Runtime.InteropServices;

namespace Das.OpenGL.Text;

[StructLayout(LayoutKind.Sequential)]
public struct FTVector26Dot6 : IEquatable<FTVector26Dot6>
{
   #region Fields

   private IntPtr x;
   private IntPtr y;

   #endregion

   #region Constructors

   /// <summary>
   /// Initializes a new instance of the <see cref="FTVector26Dot6"/> struct.
   /// </summary>
   /// <param name="x">The horizontal coordinate.</param>
   /// <param name="y">The vertical coordinate.</param>
   public FTVector26Dot6(Fixed26Dot6 x, Fixed26Dot6 y)
      : this()
   {
      this.x = (IntPtr)x.Value;
      this.y = (IntPtr)y.Value;
   }

   internal FTVector26Dot6(IntPtr reference)
      : this()
   {
      x = Marshal.ReadIntPtr(reference);
      y = Marshal.ReadIntPtr(reference, IntPtr.Size);
   }

   #endregion

   #region Properties

   /// <summary>
   /// Gets or sets the horizontal coordinate.
   /// </summary>
   public Fixed26Dot6 X
   {
      get => Fixed26Dot6.FromRawValue((Int32)x);

      set => x = (IntPtr)value.Value;
   }

   /// <summary>
   /// Gets or sets the vertical coordinate.
   /// </summary>
   public Fixed26Dot6 Y
   {
      get => Fixed26Dot6.FromRawValue((Int32)y);

      set => y = (IntPtr)value.Value;
   }

   #endregion

   #region Operators

   /// <summary>
   /// Compares two instances of <see cref="FTVector26Dot6"/> for equality.
   /// </summary>
   /// <param name="left">A <see cref="FTVector26Dot6"/>.</param>
   /// <param name="right">Another <see cref="FTVector26Dot6"/>.</param>
   /// <returns>A value indicating equality.</returns>
   public static Boolean operator ==(FTVector26Dot6 left, FTVector26Dot6 right) => left.Equals(right);

   /// <summary>
   /// Compares two instances of <see cref="FTVector26Dot6"/> for inequality.
   /// </summary>
   /// <param name="left">A <see cref="FTVector26Dot6"/>.</param>
   /// <param name="right">Another <see cref="FTVector26Dot6"/>.</param>
   /// <returns>A value indicating inequality.</returns>
   public static Boolean operator !=(FTVector26Dot6 left, FTVector26Dot6 right) => !left.Equals(right);

   #endregion

   #region Methods

   /// <summary>
   /// Compares this instance of <see cref="FTVector26Dot6"/> to another for equality.
   /// </summary>
   /// <param name="other">A <see cref="FTVector26Dot6"/>.</param>
   /// <returns>A value indicating equality.</returns>
   public Boolean Equals(FTVector26Dot6 other) => x == other.x && y == other.y;

   /// <summary>
   /// Compares this instance of <see cref="FTVector26Dot6"/> to an object for equality.
   /// </summary>
   /// <param name="obj">An object.</param>
   /// <returns>A value indicating equality.</returns>
   public override Boolean Equals(Object obj)
   {
      if (obj is FTVector26Dot6)
         return Equals((FTVector26Dot6)obj);
      else
         return false;
   }

   /// <summary>
   /// Gets a unique hash code for this instance.
   /// </summary>
   /// <returns>A hash code.</returns>
   public override Int32 GetHashCode() => x.GetHashCode() ^ y.GetHashCode();

   #endregion
}
using System;
using System.IO;
using System.IO.Compression;

namespace Das.Views.Extended.Models.Fbx;

/// <summary>
/// A wrapper for DeflateStream that calculates the Adler32 checksum of the payload
/// </summary>
public class DeflateWithChecksum : DeflateStream
{
   private const Int32 modAdler = 65521;
   private UInt32 checksumA;
   private UInt32 checksumB;

   /// <summary>
   /// Gets the Adler32 checksum at the current point in the stream
   /// </summary>
   public Int32 Checksum
   {
      get
      {
         checksumA %= modAdler;
         checksumB %= modAdler;
         return (Int32)((checksumB << 16) | checksumA);
      }
   }

   /// <inheritdoc />
   public DeflateWithChecksum(Stream stream, CompressionMode mode) : base(stream, mode)
   {
      ResetChecksum();
   }

   /// <inheritdoc />
   public DeflateWithChecksum(Stream stream, CompressionMode mode, Boolean leaveOpen) : base(stream, mode, leaveOpen)
   {
      ResetChecksum();
   }

   // Efficiently extends the checksum with the given buffer
   void CalcChecksum(Byte[] array, Int32 offset, Int32 count)
   {
      checksumA %= modAdler;
      checksumB %= modAdler;
      for (Int32 i = offset, c = 0; i < (offset + count); i++, c++)
      {
         checksumA += array[i];
         checksumB += checksumA;
         if (c > 4000) // This is about how many iterations it takes for B to reach IntMax
         {
            checksumA %= modAdler;
            checksumB %= modAdler;
            c = 0;
         }
      }
   }

   /// <inheritdoc />
   public override void Write(Byte[] array, Int32 offset, Int32 count)
   {
      base.Write(array, offset, count);
      CalcChecksum(array, offset, count);
   }

   /// <inheritdoc />
   public override Int32 Read(Byte[] array, Int32 offset, Int32 count)
   {
      var ret = base.Read(array, offset, count);
      CalcChecksum(array, offset, count);
      return ret;
   }

   /// <summary>
   /// Initializes the checksum values
   /// </summary>
   public void ResetChecksum()
   {
      checksumA = 1;
      checksumB = 0;
   }
}
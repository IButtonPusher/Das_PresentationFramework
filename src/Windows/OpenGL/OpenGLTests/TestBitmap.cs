using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Images;

namespace OpenGLTests
{
   internal class TestBitmap : IImage
   {
      public TestBitmap(Image bmp,
                        Stream? stream)
      {
         _unwrapLock = new Object();

         _bmp = bmp;
         _stream = stream;
         IsNullImage = bmp.Width == 1 && bmp.Height == 1;
      }

      Boolean IEquatable<ISize>.Equals(ISize other)
      {
         throw new NotImplementedException();
      }

      Double ISize.Height => _bmp.Height;

      Boolean ISize.IsEmpty => IsNullImage;

      Double ISize.Width => _bmp.Width;

      public Boolean HasInfiniteDimension => false;

      void IDisposable.Dispose()
      {
         if (_isDisposed)
            return;

         _isDisposed = true;
         _bmp.Dispose();

         if (_stream is { } stream)
            stream.Dispose();
      }

      public Boolean IsNullImage { get; }

      Boolean IImage.IsDisposed => _isDisposed;

      Task IImage.SaveAsync(FileInfo path)
      {
         throw new NotImplementedException();
      }

      Task IImage.SaveThenDisposeAsync(FileInfo path)
      {
         throw new NotImplementedException();
      }

      Stream? IImage.ToStream()
      {
         if (_stream != null)
            return _stream;

         var ms = new MemoryStream();
         _bmp.Save(ms, ImageFormat.Png);
         ms.Position = 0;
         return ms;
      }

      Task<Boolean> IImage.TrySave(FileInfo path)
      {
         throw new NotImplementedException();
      }

      T IImage.Unwrap<T>()
      {
         if (_bmp is T good)
            return good;

         throw new NotImplementedException();
      }


      public void UnwrapLocked<T>(Action<T> action)
      {
         lock (_unwrapLock)
         {
            if (_bmp is T good)
               action(good);
            else
               throw new NotImplementedException();
         }
      }

      Task<TResult> IImage.UseImage<TImage, TParam, TResult>(TParam param1,
                                                             Func<TImage, TParam, TResult> action)
      {
         throw new NotImplementedException();
      }

      private readonly Image _bmp;
      private readonly Stream? _stream;
      private readonly Object _unwrapLock;

      private Boolean _isDisposed;
   }
}

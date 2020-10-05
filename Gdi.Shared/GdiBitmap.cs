using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Gdi.Shared
{
    public class GdiBitmap: IImage
    {
        private readonly Bitmap _bmp;

        public GdiBitmap(Bitmap bmp)
        {
            _bmp = bmp;
        }

        private Boolean _isDisposed;

        Boolean IEquatable<ISize>.Equals(ISize other)
        {
            throw new NotImplementedException();
        }

        Double ISize.Height => _bmp.Height;

        Boolean ISize.IsEmpty => false;

        Double ISize.Width => _bmp.Width;

        void IDisposable.Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            _bmp.Dispose();
        }

        Boolean IImage.IsDisposed => _isDisposed;

        Task IImage.SaveAsync(FileInfo path)
        {
            throw new NotImplementedException();
        }

        Task IImage.SaveThenDisposeAsync(FileInfo path)
        {
            throw new NotImplementedException();
        }

        Stream IImage.ToStream()
        {
            throw new NotImplementedException();
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

        Task<TResult> IImage.UseImage<TImage, TParam, TResult>(TParam param1, 
                                                               Func<TImage, TParam, TResult> action)
        {
            throw new NotImplementedException();
        }
    }
}

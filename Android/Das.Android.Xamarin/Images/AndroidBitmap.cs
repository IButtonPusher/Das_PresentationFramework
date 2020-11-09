using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Das.Views;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Xamarin.Android
{
    public class AndroidBitmap : IImage
    {
        private readonly Bitmap _bmp;
        private readonly Boolean _isEmpty;

        public AndroidBitmap(Bitmap bmp)
        {
            _bmp = bmp;
            _isEmpty = bmp == null || 
                       (bmp.Width == 1 && bmp.Height == 1);
        }

        private Boolean _isDisposed;

        Boolean IEquatable<ISize>.Equals(ISize other)
        {
            return _bmp.Width == Convert.ToInt32(other.Width) && 
                   _bmp.Height == Convert.ToInt32(other.Height);
        }

        Double ISize.Height => _bmp.Height;

        Boolean ISize.IsEmpty => _isEmpty;

        Double ISize.Width => _bmp.Width;

        void IDisposable.Dispose()
        {
            if (_isDisposed)
                return;
            _bmp.Dispose();
            _isDisposed = true;
        }

        ISize IDeepCopyable<ISize>.DeepCopy()
        {
            return new ValueSize(_bmp.Width, _bmp.Height);
        }

        public ISize Reduce(Thickness padding)
        {
            return GeometryHelper.Reduce(this, padding);
        }

        ISize ISize.Minus(ISize subtract)
        {
            return GeometryHelper.Minus(this, subtract);
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
            var ms = new MemoryStream();
            if (!_isEmpty)
                _bmp.Compress(Bitmap.CompressFormat.Png, 0, ms);
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

        Task<TResult> IImage.UseImage<TImage, TParam, TResult>(TParam param1, 
                                                               Func<TImage, TParam, TResult> action)
        {
            throw new NotImplementedException();
        }
    }
}
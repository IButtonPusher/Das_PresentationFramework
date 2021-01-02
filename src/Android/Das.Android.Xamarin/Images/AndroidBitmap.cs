﻿using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Xamarin.Android
{
    public class AndroidBitmap : IImage
    {

        public AndroidBitmap(Bitmap bmp,
                             Stream? stream)
        {
            _unwrapLock = new Object();

            _bmp = bmp!;
            _stream = stream;
            _isEmpty = bmp.Width == 1 && bmp.Height == 1;
        }

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

            if (_bmp != null)
                _bmp.Dispose();
            _isDisposed = true;
        }

        public Boolean IsNullImage => _isEmpty;

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
            if (_stream != null && _stream.CanRead)
            {
                return _stream;
            }

            var ms = new MemoryStream();
            if (!_isEmpty)
                _bmp.Compress(Bitmap.CompressFormat.Png, 0, ms);
            ms.Position = 0;
            return ms;
        }

        public ISize PlusVertical(ISize adding)
        {
            return GeometryHelper.PlusVertical(this, adding);
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

        //ISize ISize.Divide(Double pct)
        //{
        //    return GeometryHelper.Divide(this, pct);
        //}

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

        public Double CenterY(ISize item)
        {
            return GeometryHelper.CenterY(this, item);
        }

        public Double CenterX(ISize item)
        {
            return GeometryHelper.CenterX(this, item);
        }


        Task<TResult> IImage.UseImage<TImage, TParam, TResult>(TParam param1,
                                                               Func<TImage, TParam, TResult> action)
        {
            throw new NotImplementedException();
        }

        private readonly Bitmap _bmp;
        private readonly Stream? _stream;
        private readonly Boolean _isEmpty;
        private readonly Object _unwrapLock;

        private Boolean _isDisposed;
    }
}
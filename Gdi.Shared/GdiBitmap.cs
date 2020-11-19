using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Gdi.Shared
{
    public class GdiBitmap: IImage
    {
        private readonly Image _bmp;
        private readonly Stream? _stream;
        private readonly Boolean _isEmpty;

        //private static Int32 _count;

        public GdiBitmap(Image bmp,
                         Stream? stream)
        {
            //Debug.WriteLine("Create bmp " + (++_count));

            _bmp = bmp;
            _stream = stream;
            _isEmpty = bmp.Width == 1 && bmp.Height == 1;
        }

        private Boolean _isDisposed;

        Boolean IEquatable<ISize>.Equals(ISize other)
        {
            throw new NotImplementedException();
        }

        Double ISize.Height => _bmp.Height;

        Boolean ISize.IsEmpty => _isEmpty;

        Double ISize.Width => _bmp.Width;

        public ISize PlusVertical(ISize adding)
        {
            return GeometryHelper.PlusVertical(this, adding);
        }

        public ISize Reduce(Thickness padding)
        {
            return GeometryHelper.Reduce(this, padding);
        }

        ISize ISize.Minus(ISize subtract)
        {
            return GeometryHelper.Minus(this, subtract);
        }

        void IDisposable.Dispose()
        {
            if (_isDisposed)
                return;

            //Debug.WriteLine("Dispose bmp " + (--_count));

            _isDisposed = true;
            _bmp.Dispose();

            if (_stream is {} stream)
                stream.Dispose();
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

        Stream? IImage.ToStream()
        {
            return _stream;
            //var ms = new MemoryStream();
            //_bmp.Save(ms, ImageFormat.Png);
            //ms.Position = 0;
            //return ms;
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

        ISize IDeepCopyable<ISize>.DeepCopy()
        {
            return new ValueSize(_bmp.Width, _bmp.Height);
        }
    }
}

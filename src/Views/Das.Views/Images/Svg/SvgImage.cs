using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Images.Svg;

namespace Das.Views.Images
{
    public class SvgImage : IImage
    {
        public SvgImage(Double width,
                        Double height,
                        IEnumerable<SvgPathSegment> segments,
                        IImageProvider imageProvider)
        {
            _imageProvider = imageProvider;
            Width = width;
            Height = height;
            Segments = new List<SvgPathSegment>(segments);

            Stroke = Color.Black;
        }

        public Double Height { get; }

        public Boolean IsEmpty => throw new NotImplementedException();


        public Double Width { get; }

        public IColor Stroke { get; set; }

        public IBrush? Fill { get; set; }


        public Boolean Equals(ISize other)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Boolean IsDisposed => throw new NotImplementedException();

        public Boolean IsNullImage => false;

        public Task SaveAsync(FileInfo path)
        {
            throw new NotImplementedException();
        }

        public Task SaveThenDisposeAsync(FileInfo path)
        {
            throw new NotImplementedException();
        }

        public Stream? ToStream()
        {
            throw new NotImplementedException();
        }

        public Task<Boolean> TrySave(FileInfo path)
        {
            throw new NotImplementedException();
        }

        public T Unwrap<T>()
        {
            if (_cachedUnwrapped is T good)
                return good;

            using (var path = _imageProvider.GetNewGraphicsPath())
            {
                AddToPath(path);

                var cooked = _imageProvider.GetImage(path, Stroke);
                var img = cooked.Unwrap<T>();
                _cachedUnwrapped = img;

                return img;

            }
        }

        public void UnwrapLocked<T>(Action<T> action)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> UseImage<TImage, TParam, TResult>(TParam param1,
                                                               Func<TImage, TParam, TResult> action)
        {
            throw new NotImplementedException();
        }

        public List<SvgPathSegment> Segments { get; }

        public void AddToPath(IGraphicsPath graphicsPath)
        {
            foreach (var s in Segments)
            {
                s.AddToPath(graphicsPath);
            }
        }

        private readonly IImageProvider _imageProvider;
        private Object? _cachedUnwrapped;
    }
}

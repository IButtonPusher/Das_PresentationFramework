using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Images.Svg;
using Das.Views.Transforms;

namespace Das.Views.Images
{
    public class SvgImage : ISvgImage
    {
        public SvgImage(Double width,
                        Double height,
                        IEnumerable<SvgPathSegment> segments,
                        IImageProvider imageProvider)
        {
            _unwrapLock = new Object();

            _imageProvider = imageProvider;
            Width = width;
            Height = height;
            Segments = new List<SvgPathSegment>(segments);

            Stroke = Color.Black;

            HasInfiniteDimension = Double.IsInfinity(Width) || Double.IsInfinity(Height);
        }

        public Double Height { get; }

        public Boolean IsEmpty => throw new NotImplementedException();


        public Double Width { get; }

        public Boolean HasInfiniteDimension { get; }

        public IColor? Stroke { get; set; }

        public IBrush? Fill { get; set; }

        public IImage? ToStaticImage(IColor? stroke,
                                     IBrush? fill)
        {
            return ToStaticImage(Convert.ToInt32(Width), Convert.ToInt32(Height), 
                stroke, fill);
        }

        public IImage? ToStaticImage(Int32 width,
                                     Int32 height,
                                     IColor? stroke,
                                     IBrush? fill)
        {
           if (width.IsZero() || height.IsZero())
              return default;

           using (var path = _imageProvider.GetNewGraphicsPath())
           {
              AddToPath(path);

              if (Width.AreDifferent(width) ||
                  Height.AreDifferent(height))
              {
                 var scaleX = width / Width;
                 var scaleY = height / Height;

                 var xformMatrix = new TransformationMatrix(scaleX, 0, 0, scaleY, 0, 0);
                 path.Transform(xformMatrix);
              }

              var cooked = path.ToImage(width,
                 height, stroke, fill);

              return cooked;
           }
        }


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

            if (!(Stroke is { } stroke))
                return default!;

            var cooked = ToStaticImage(stroke, Fill);
            if (cooked == null)
                return default!;

            var img = cooked.Unwrap<T>();
            _cachedUnwrapped = img;
            return img;
        }

        public void UnwrapLocked<T>(Action<T> action)
        {
            lock (_unwrapLock)
            {
                var u = Unwrap<T>();
                action(u);
            }
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
        private readonly Object _unwrapLock;
    }
}

using System;
using System.IO;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Views.Images
{
    public class NullImage : IImage
    {
        public static NullImage Instance = new NullImage();

        #if NET40
        private static readonly Task _completedTask = TaskEx.CompletedTask;
        #else
        private static readonly Task _completedTask = Task.CompletedTask;
        #endif

        private NullImage()
        {}

        public Boolean Equals(ISize other)
        {
            return other.IsEmpty;
        }

        public ISize DeepCopy()
        {
            return ValueSize.Empty;
        }

        public Double Height => 0;

        public Boolean IsEmpty => true;

        public Double Width => 0;

        public ISize Minus(ISize subtract)
        {
            return new ValueSize(0 - subtract.Width, 0 - subtract.Height);
        }

        public ISize Divide(Double pct)
        {
            return GeometryHelper.Divide(ValueSize.Empty, pct);
        }

        public ISize PlusVertical(ISize adding)
        {
            return GeometryHelper.PlusVertical(ValueSize.Empty, adding);
        }

        public ISize Reduce(Thickness padding)
        {
            return GeometryHelper.Reduce(ValueSize.Empty, padding);
        }

        public Double CenterY(ISize item)
        {
            return GeometryHelper.CenterY(ValueSize.Empty, item);
        }

        public Double CenterX(ISize item)
        {
            return GeometryHelper.CenterX(ValueSize.Empty, item);
        }

        public void Dispose()
        {
            
        }

        public Boolean IsNullImage => true;

        public Boolean IsDisposed => true;

        public Task SaveAsync(FileInfo path)
        {
            return _completedTask;
        }

        public Task SaveThenDisposeAsync(FileInfo path)
        {
            return _completedTask;
        }

        public Stream? ToStream()
        {
            return null;
        }

        public Task<Boolean> TrySave(FileInfo path)
        {
            #if NET40

            return TaskEx.FromResult<Boolean>(false);

            #else

            return Task.FromResult<Boolean>(false);

            #endif
        }

        T IImage.Unwrap<T>()
        {
            throw new NotSupportedException();
        }

        void IImage.UnwrapLocked<T>(Action<T> action)
        {
            throw new NotSupportedException();
        }

        Task<TResult> IImage.UseImage<TImage, TParam, TResult>(TParam param1, 
                                                               Func<TImage, TParam, TResult> action)
        {
            throw new NotSupportedException();
        }

       
    }
}

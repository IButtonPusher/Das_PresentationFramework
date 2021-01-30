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

        public Double Height => 0;

        public Boolean IsEmpty => true;

        public Double Width => 0;

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

            return Task.FromResult(false);

            #endif
        }

        T IImage.Unwrap<T>()
        {
            throw new NotSupportedException();
        }

        public Int64 SizeInBytes => 0;

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

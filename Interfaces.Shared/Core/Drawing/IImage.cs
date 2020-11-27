using System;
using System.IO;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Core.Drawing
{
    public interface IImage : ISize, IDisposable
    {
        Boolean IsDisposed { get; }

        Task SaveAsync(FileInfo path);

        Task SaveThenDisposeAsync(FileInfo path);

        Stream? ToStream();

        Task<Boolean> TrySave(FileInfo path);

        T Unwrap<T>();

        void UnwrapLocked<T>(Action<T> action);

        Task<TResult> UseImage<TImage, TParam, TResult>(
            TParam param1, Func<TImage, TParam, TResult> action);
    }
}
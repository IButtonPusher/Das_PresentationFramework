using System;
using System.IO;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Core.Drawing
{
    public interface IImage : ISize, IDisposable
    {
        T Unwrap<T>();

        Task<Boolean> TrySave(FileInfo path);

        Task SaveAsync(FileInfo path);

        Task SaveThenDisposeAsync(FileInfo path);

        Task<TResult> UseImage<TImage, TParam, TResult>(
            TParam param1, Func<TImage, TParam, TResult> action);

        Stream ToStream();

        Boolean IsDisposed { get; }
    }
}
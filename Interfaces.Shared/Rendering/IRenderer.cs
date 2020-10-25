using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    /// <summary>
    ///     Performs a render operation and provides a TAsset that can be
    ///     used by an IViewHost for system specific rendering
    /// </summary>
    /// <example>TAsset as a Bitmap can be drawn in an OnPaint method</example>
    /// <typeparam name="TAsset">The fully cooked, system specific, object</typeparam>
    public interface IRenderer<out TAsset>
    {
        TAsset DoRender();

        ISize? GetContentSize(ISize available);

        event EventHandler? Rendering;
    }

    public interface IRenderer
    {
        void DoRender();

        void Initialize();
    }
}
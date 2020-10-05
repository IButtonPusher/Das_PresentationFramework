using System;
using System.Threading.Tasks;

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

        event EventHandler? Rendering;
    }

    public interface IRenderer
    {
        void DoRender();

        void Initialize();
    }
}
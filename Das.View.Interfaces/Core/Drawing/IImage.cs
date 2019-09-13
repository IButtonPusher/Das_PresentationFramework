using Das.Views.Core.Geometry;

namespace Das.Views.Core.Drawing
{
    public interface IImage : ISize
    {
        T Unwrap<T>();
    }
}
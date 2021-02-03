using Das.Views.Core.Drawing;

namespace Das.Views.Images
{
    public interface ISvgImage : IImage
    {
        IColor? Stroke { get; }

        IBrush? Fill { get; }

        IImage ToStaticImage(IColor? stroke,
                              IBrush? fill);

        void AddToPath(IGraphicsPath graphicsPath);
    }
}

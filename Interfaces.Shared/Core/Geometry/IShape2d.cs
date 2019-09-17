namespace Das.Views.Core.Geometry
{
    public interface IShape2d : ISize
    {
        double Left { get; }

        double Top { get; }

        double Right { get; }

        double Bottom { get; }
    }
}
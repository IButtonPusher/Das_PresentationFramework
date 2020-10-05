using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
    public class Cube : Rectangle, ISize3d, ICube
    {
        public Cube(Point2D location, ISize3d size)
            : base(location, size)
        {
            Depth = size.Depth;
        }

        public Cube(Rectangle start, Double depth) : base(start, Thickness.Empty)
        {
            Depth = depth;
        }

        public Cube(Rectangle start, Thickness margin, Double depth) : base(start, margin)
        {
            Depth = depth;
        }

        public Cube(Double x, Double y, Double width, Double height, Double depth)
            : base(x, y, width, height)
        {
            Depth = depth;
        }

        public Double Depth { get; }
    }
}
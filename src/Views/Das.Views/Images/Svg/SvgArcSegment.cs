using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Views.Images.Svg
{
    /// <summary>
    ///     https://github.com/svg-net
    /// </summary>
    public sealed class SvgArcSegment : SvgPathSegment
    {
        public SvgArcSegment(IPoint2F start,
                             Single radiusX,
                             Single radiusY,
                             Single angle,
                             SvgArcSize size,
                             SvgArcSweep sweep,
                             IPoint2F end)
            : base(start, end)
        {
            RadiusX = Math.Abs(radiusX);
            RadiusY = Math.Abs(radiusY);
            Angle = angle;
            Sweep = sweep;
            Size = size;
        }

        public Single Angle { get; set; }

        public Single RadiusX { get; set; }

        public Single RadiusY { get; set; }

        public SvgArcSize Size { get; set; }

        public SvgArcSweep Sweep { get; set; }

        public override void AddToPath(IGraphicsPath graphicsPath)
        {
            if (Start == End)
                return;

            if (RadiusX == 0.0f && RadiusY == 0.0f)
            {
                graphicsPath.AddLine(Start, End);
                return;
            }

            var sinPhi = Math.Sin(Angle * RadiansPerDegree);
            var cosPhi = Math.Cos(Angle * RadiansPerDegree);

            var x1dash = cosPhi * (Start.X - End.X) / 2.0 + sinPhi * (Start.Y - End.Y) / 2.0;
            var y1dash = -sinPhi * (Start.X - End.X) / 2.0 + cosPhi * (Start.Y - End.Y) / 2.0;

            Double root;
            var numerator = RadiusX * RadiusX * RadiusY * RadiusY - RadiusX * RadiusX * y1dash * y1dash -
                            RadiusY * RadiusY * x1dash * x1dash;

            var rx = RadiusX;
            var ry = RadiusY;

            if (numerator < 0.0)
            {
                var s = (Single) Math.Sqrt(1.0 - numerator / (RadiusX * RadiusX * RadiusY * RadiusY));

                rx *= s;
                ry *= s;
                root = 0.0;
            }
            else
                root = (Size == SvgArcSize.Large && Sweep == SvgArcSweep.Positive ||
                        Size == SvgArcSize.Small && Sweep == SvgArcSweep.Negative
                    ? -1.0
                    : 1.0) * Math.Sqrt(numerator /
                                       (RadiusX * RadiusX * y1dash * y1dash + RadiusY * RadiusY * x1dash * x1dash));

            var cxdash = root * rx * y1dash / ry;
            var cydash = -root * ry * x1dash / rx;

            var cx = cosPhi * cxdash - sinPhi * cydash + (Start.X + End.X) / 2.0;
            var cy = sinPhi * cxdash + cosPhi * cydash + (Start.Y + End.Y) / 2.0;

            var theta1 = CalculateVectorAngle(1.0, 0.0, (x1dash - cxdash) / rx, (y1dash - cydash) / ry);
            var dtheta = CalculateVectorAngle((x1dash - cxdash) / rx, (y1dash - cydash) / ry, (-x1dash - cxdash) / rx,
                (-y1dash - cydash) / ry);

            if (Sweep == SvgArcSweep.Negative && dtheta > 0)
                dtheta -= 2.0 * Math.PI;
            else if (Sweep == SvgArcSweep.Positive && dtheta < 0)
                dtheta += 2.0 * Math.PI;

            var segments = (Int32) Math.Ceiling(Math.Abs(dtheta / (Math.PI / 2.0)));
            var delta = dtheta / segments;
            var t = 8.0 / 3.0 * Math.Sin(delta / 4.0) * Math.Sin(delta / 4.0) / Math.Sin(delta / 2.0);

            var startX = Start.X;
            var startY = Start.Y;

            for (var i = 0; i < segments; ++i)
            {
                var cosTheta1 = Math.Cos(theta1);
                var sinTheta1 = Math.Sin(theta1);
                var theta2 = theta1 + delta;
                var cosTheta2 = Math.Cos(theta2);
                var sinTheta2 = Math.Sin(theta2);

                var endpointX = cosPhi * rx * cosTheta2 - sinPhi * ry * sinTheta2 + cx;
                var endpointY = sinPhi * rx * cosTheta2 + cosPhi * ry * sinTheta2 + cy;

                var dx1 = t * (-cosPhi * rx * sinTheta1 - sinPhi * ry * cosTheta1);
                var dy1 = t * (-sinPhi * rx * sinTheta1 + cosPhi * ry * cosTheta1);

                var dxe = t * (cosPhi * rx * sinTheta2 + sinPhi * ry * cosTheta2);
                var dye = t * (sinPhi * rx * sinTheta2 - cosPhi * ry * cosTheta2);

                graphicsPath.AddBezier(startX, startY, (Single) (startX + dx1), (Single) (startY + dy1),
                    (Single) (endpointX + dxe), (Single) (endpointY + dye), (Single) endpointX, (Single) endpointY);

                theta1 = theta2;
                startX = (Single) endpointX;
                startY = (Single) endpointY;
            }
        }

        public override String ToString()
        {
            var arcFlag = Size == SvgArcSize.Large ? "1" : "0";
            var sweepFlag = Sweep == SvgArcSweep.Positive ? "1" : "0";
            return "A" + RadiusX.ToSvgString() + " " + RadiusY.ToSvgString() + " " +
                   Angle.ToSvgString() + " " +
                   arcFlag + " " + sweepFlag + " " + End.ToSvgString();
        }

        private static Double CalculateVectorAngle(Double ux,
                                                   Double uy,
                                                   Double vx,
                                                   Double vy)
        {
            var ta = Math.Atan2(uy, ux);
            var tb = Math.Atan2(vy, vx);

            if (tb >= ta)
                return tb - ta;

            return DoublePI - (ta - tb);
        }

        private const Double RadiansPerDegree = Math.PI / 180.0;
        private const Double DoublePI = Math.PI * 2;
    }

    [Flags]
    public enum SvgArcSweep
    {
        Negative = 0,
        Positive = 1
    }

    [Flags]
    public enum SvgArcSize
    {
        Small = 0,
        Large = 1
    }
}

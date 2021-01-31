using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Core.Geometry;

namespace Das.Views.Images.Svg
{
    /// <summary>
    ///     https://github.com/svg-net
    /// </summary>
    public class SvgPathBuilder //: TypeConverter
    {
        private readonly IImageProvider _imageProvider;

        public SvgPathBuilder(IImageProvider imageProvider)
        {
            _imageProvider = imageProvider;
        }

        //public override Object ConvertFrom(ITypeDescriptorContext context,
        //                                   CultureInfo culture,
        //                                   Object value)
        //{
        //    if (value is String s)
        //        return Parse(s.AsSpan());

        //    return base.ConvertFrom(context, culture, value)!;
        //}

        public SvgImage Parse(SvgDocument doc)
        {
            var path = doc.Path?.D ?? throw new NullReferenceException();
            return Parse(doc.Width, doc.Height, path.AsSpan());
        }

        //public static SvgImage Parse(String path)
        //{
        //    return Parse(path.AsSpan());
        //}

        
        private SvgImage Parse(Double width,
                                      Double height,
                                      ReadOnlySpan<Char> path)
        {
            var segments = new SvgPathSegmentList();

            try
            {
                var pathTrimmed = path.TrimEnd();
                var commandStart = 0;
                var pathLength = pathTrimmed.Length;

                for (var i = 0; i < pathLength; ++i)
                {
                    var currentChar = pathTrimmed[i];
                    if (Char.IsLetter(currentChar) && currentChar != 'e' && currentChar != 'E'
                    ) // e is used in scientific notiation. but not svg path
                    {
                        var start = commandStart;
                        var length = i - commandStart;
                        var command = pathTrimmed.Slice(start, length).Trim();
                        commandStart = i;

                        if (command.Length > 0)
                        {
                            var commandSetTrimmed = pathTrimmed.Slice(start, length).Trim();
                            var state = new CoordinateParserState(ref commandSetTrimmed);
                            CreatePathSegments(commandSetTrimmed[0], segments, state, commandSetTrimmed);
                        }

                        if (pathLength == i + 1)
                        {
                            var commandSetTrimmed = pathTrimmed.Slice(i, 1).Trim();
                            var state = new CoordinateParserState(ref commandSetTrimmed);
                            CreatePathSegments(commandSetTrimmed[0], segments, state, commandSetTrimmed);
                        }
                    }
                    else if (pathLength == i + 1)
                    {
                        var start = commandStart;
                        var length = i - commandStart + 1;
                        var command = pathTrimmed.Slice(start, length).Trim();

                        if (command.Length > 0)
                        {
                            var commandSetTrimmed = pathTrimmed.Slice(start, length).Trim();
                            var state = new CoordinateParserState(ref commandSetTrimmed);
                            CreatePathSegments(commandSetTrimmed[0], segments, state, commandSetTrimmed);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Trace.TraceError("Error parsing path \"{0}\": {1}", path.ToString(), exc.Message);
            }

            return new SvgImage(width, height, segments, _imageProvider);
        }

        private static //IEnumerable<SvgPathSegment> 
            void CreatePathSegments(Char command,
                                                                     SvgPathSegmentList segments,
                                                                     CoordinateParserState state,
                                                                     ReadOnlySpan<Char> chars)
        {
            var isRelative = Char.IsLower(command);
            // http://www.w3.org/TR/SVG11/paths.html#PathDataGeneralInformation

            switch (command)
            {
                case 'M': // moveto
                case 'm': // relative moveto
                {
                    if (CoordinateParser.TryGetFloat(out var coords0, ref chars, ref state)
                        && CoordinateParser.TryGetFloat(out var coords1, ref chars, ref state))
                    {
                        var mov = new SvgMoveToSegment(
                            ToAbsolute(coords0, coords1, segments, isRelative));

                        segments.Add(mov);
                        //yield return mov;
                    }

                    while (CoordinateParser.TryGetFloat(out coords0, ref chars, ref state)
                           && CoordinateParser.TryGetFloat(out coords1, ref chars, ref state))
                    {
                        var line = new SvgLineSegment(segments.Last.End,
                            ToAbsolute(coords0, coords1, segments, isRelative));

                        segments.Add(line);

                        //yield return line;
                    }
                }
                    break;
                case 'A': // elliptical arc
                case 'a': // relative elliptical arc
                {
                    while (CoordinateParser.TryGetFloat(out var coords0, ref chars, ref state)
                           && CoordinateParser.TryGetFloat(out var coords1, ref chars, ref state)
                           && CoordinateParser.TryGetFloat(out var coords2, ref chars, ref state)
                           && CoordinateParser.TryGetBool(out var size, ref chars, ref state)
                           && CoordinateParser.TryGetBool(out var sweep, ref chars, ref state)
                           && CoordinateParser.TryGetFloat(out var coords3, ref chars, ref state)
                           && CoordinateParser.TryGetFloat(out var coords4, ref chars, ref state))
                    {
                        // A|a rx ry x-axis-rotation large-arc-flag sweep-flag x y

                        var arc = new SvgArcSegment(
                            segments.Last.End,
                            coords0,
                            coords1,
                            coords2,
                            size ? SvgArcSize.Large : SvgArcSize.Small,
                            sweep ? SvgArcSweep.Positive : SvgArcSweep.Negative,
                            ToAbsolute(coords3, coords4, segments, isRelative));
                        
                        segments.Add(arc);

                        //yield return arc;
                    }
                }
                    break;
                case 'L': // lineto
                case 'l': // relative lineto
                {
                    while (CoordinateParser.TryGetFloat(out var coords0, ref chars, ref state)
                           && CoordinateParser.TryGetFloat(out var coords1, ref chars, ref state))
                    {
                        var line = new SvgLineSegment(
                            segments.Last.End,
                            ToAbsolute(coords0, coords1, segments, isRelative));

                        segments.Add(line);
                        //yield return line;
                    }
                }
                    break;
                case 'H': // horizontal lineto
                case 'h': // relative horizontal lineto
                {
                    while (CoordinateParser.TryGetFloat(out var coords0, ref chars, ref state))
                    {
                        var line = new SvgLineSegment(
                            segments.Last.End,
                            ToAbsolute(coords0, segments.Last.End.Y, segments, isRelative, false));

                        segments.Add(line);

                        //yield return line;
                    }
                }
                    break;
                case 'V': // vertical lineto
                case 'v': // relative vertical lineto
                {
                    while (CoordinateParser.TryGetFloat(out var coords0, ref chars, ref state))
                    {
                        var line = new SvgLineSegment(
                            segments.Last.End,
                            ToAbsolute(segments.Last.End.X, coords0, segments, false, isRelative));

                        segments.Add(line);
                        //yield return line;
                    }
                }
                    break;
                case 'Q': // quadratic bézier curveto
                case 'q': // relative quadratic bézier curveto
                {
                    while (CoordinateParser.TryGetFloat(out var coords0, ref chars, ref state)
                           && CoordinateParser.TryGetFloat(out var coords1, ref chars, ref state)
                           && CoordinateParser.TryGetFloat(out var coords2, ref chars, ref state)
                           && CoordinateParser.TryGetFloat(out var coords3, ref chars, ref state))
                    {
                        var quad = new SvgQuadraticCurveSegment(
                            segments.Last.End,
                            ToAbsolute(coords0, coords1, segments, isRelative),
                            ToAbsolute(coords2, coords3, segments, isRelative));

                        segments.Add(quad);
                        //yield return quad;
                    }
                }
                    break;
                case 'T': // shorthand/smooth quadratic bézier curveto
                case 't': // relative shorthand/smooth quadratic bézier curveto
                {
                    while (CoordinateParser.TryGetFloat(out var coords0, ref chars, ref state)
                           && CoordinateParser.TryGetFloat(out var coords1, ref chars, ref state))
                    {
                        var lastQuadCurve = segments.Last as SvgQuadraticCurveSegment;
                        var controlPoint = lastQuadCurve != null
                            ? Reflect(lastQuadCurve.ControlPoint, segments.Last.End)
                            : segments.Last.End;

                        var quad = new SvgQuadraticCurveSegment(
                            segments.Last.End,
                            controlPoint,
                            ToAbsolute(coords0, coords1, segments, isRelative));

                        segments.Add(quad);
                        //yield return quad;
                    }
                }
                    break;
                case 'C': // curveto
                case 'c': // relative curveto
                {
                    while (CoordinateParser.TryGetFloat(out var coords0, ref chars, ref state)
                           && CoordinateParser.TryGetFloat(out var coords1, ref chars, ref state)
                           && CoordinateParser.TryGetFloat(out var coords2, ref chars, ref state)
                           && CoordinateParser.TryGetFloat(out var coords3, ref chars, ref state)
                           && CoordinateParser.TryGetFloat(out var coords4, ref chars, ref state)
                           && CoordinateParser.TryGetFloat(out var coords5, ref chars, ref state))
                    {
                        var cube = new SvgCubicCurveSegment(
                            segments.Last.End,
                            ToAbsolute(coords0, coords1, segments, isRelative),
                            ToAbsolute(coords2, coords3, segments, isRelative),
                            ToAbsolute(coords4, coords5, segments, isRelative));

                        segments.Add(cube);
                        //yield return cube;
                    }
                }
                    break;
                case 'S': // shorthand/smooth curveto
                case 's': // relative shorthand/smooth curveto
                {
                    while (CoordinateParser.TryGetFloat(out var coords0, ref chars, ref state)
                           && CoordinateParser.TryGetFloat(out var coords1, ref chars, ref state)
                           && CoordinateParser.TryGetFloat(out var coords2, ref chars, ref state)
                           && CoordinateParser.TryGetFloat(out var coords3, ref chars, ref state))
                    {
                        var lastCubicCurve = segments.Last as SvgCubicCurveSegment;
                        var controlPoint = lastCubicCurve != null
                            ? Reflect(lastCubicCurve.SecondControlPoint, segments.Last.End)
                            : segments.Last.End;

                        var cube = new SvgCubicCurveSegment(
                            segments.Last.End,
                            controlPoint,
                            ToAbsolute(coords0, coords1, segments, isRelative),
                            ToAbsolute(coords2, coords3, segments, isRelative));

                        segments.Add(cube);
                        //yield return cube;
                    }
                }
                    break;
                case 'Z': // closepath
                case 'z': // relative closepath
                {
                    var bye = new SvgClosePathSegment();
                    segments.Add(bye);
                    //yield return bye;
                }
                    break;
            }
        }

        private static IPoint2F Reflect(IPoint2F point,
                                        IPoint2F mirror)
        {
            var dx = Math.Abs(mirror.X - point.X);
            var dy = Math.Abs(mirror.Y - point.Y);

            var x = mirror.X + (mirror.X >= point.X ? dx : -dx);
            var y = mirror.Y + (mirror.Y >= point.Y ? dy : -dy);

            return new ValuePoint2F(x, y);
        }

        /// <summary>
        ///     Creates point with absolute coorindates.
        /// </summary>
        /// <param name="x">Raw X-coordinate value.</param>
        /// <param name="y">Raw Y-coordinate value.</param>
        /// <param name="segments">Current path segments.</param>
        /// <param name="isRelativeBoth">
        ///     <b>true</b> if <paramref name="x" /> and <paramref name="y" /> contains relative
        ///     coordinate values, otherwise <b>false</b>.
        /// </param>
        /// <returns><see cref="IPoint2F" /> that contains absolute coordinates.</returns>
        private static IPoint2F ToAbsolute(Single x,
                                           Single y,
                                           SvgPathSegmentList segments,
                                           Boolean isRelativeBoth)
        {
            return ToAbsolute(x, y, segments, isRelativeBoth, isRelativeBoth);
        }

        /// <summary>
        ///     Creates point with absolute coorindates.
        /// </summary>
        /// <param name="x">Raw X-coordinate value.</param>
        /// <param name="y">Raw Y-coordinate value.</param>
        /// <param name="segments">Current path segments.</param>
        /// <param name="isRelativeX">
        ///     <b>true</b> if <paramref name="x" /> contains relative coordinate value, otherwise
        ///     <b>false</b>.
        /// </param>
        /// <param name="isRelativeY">
        ///     <b>true</b> if <paramref name="y" /> contains relative coordinate value, otherwise
        ///     <b>false</b>.
        /// </param>
        /// <returns><see cref="IPoint2F" /> that contains absolute coordinates.</returns>
        private static IPoint2F ToAbsolute(Single x,
                                           Single y,
                                           SvgPathSegmentList segments,
                                           Boolean isRelativeX,
                                           Boolean isRelativeY)
        {
            if ((isRelativeX || isRelativeY) && segments.Count > 0)
            {
                var lastSegment = segments.Last;

                // if the last element is a SvgClosePathSegment the position of the previous element should be used because the position of SvgClosePathSegment is 0,0
                if (lastSegment is SvgClosePathSegment && segments.Count > 0)
                    for (var i = segments.Count - 1; i >= 0; i--)
                        if (segments[i] is SvgMoveToSegment moveToSegment)
                        {
                            lastSegment = moveToSegment;
                            break;
                        }

                if (isRelativeX)
                    x += lastSegment.End.X;
                //point.X += lastSegment.End.X;

                if (isRelativeY)
                    y += lastSegment.End.Y;
                //point.Y += lastSegment.End.Y;
            }

            var point = new ValuePoint2F(x, y);

            return point;
        }
    }
}

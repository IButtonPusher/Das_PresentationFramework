using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Images.Svg;
using Gdi.Shared;
using Xunit;

namespace Dpf.Tests
{
    public class SvgTests : TestBase
    {
        [Fact]
        public void DeserializeSvgXml()
        {
            var settings = DasSettings.CloneDefault();
            settings.IsPropertyNamesCaseSensitive = false;
            
            var srl = new DasSerializer(settings);

            var xml = GetFileContents("cog.svg");

            //var fullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            //    "Files",
            //    "cog.svg");

            //var fi = new FileInfo(fullName);

            var res = srl.FromXml<SvgDocument>(xml);

            var bldr = new SvgPathBuilder(new TestImageProvider(), srl);

            //var bob = SvgPathBuilder.Parse(res.Path.D);
            var bob = bldr.Parse(res);

            var gpath = new GdiGraphicsPath();
            gpath.Path.FillMode = FillMode.Winding;
            bob.AddToPath(gpath);

            //using (var bmp = new Bitmap(200, 200))
            using (var bmp = new Bitmap(48, 48))
            {
                var path = gpath.Path;
                
                //var pBounds = path.GetBounds();
                var scaleX = bmp.Width / (Single)res.Width;
                var scaleY = bmp.Height / (Single) res.Height;

                Matrix m = new Matrix();
                m.Scale(scaleX, scaleY, MatrixOrder.Append);
                //m.Translate(offsetX, offsetY, MatrixOrder.Append);
                path.Transform(m);

                //using (var g = Graphics.FromImage(bmp))
                using (var g = bmp.GetSmoothGraphics())
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    g.Clear(Color.AliceBlue);
                    g.FillPath(Brushes.Black, gpath.Path);

                    //gpath.Path.Transform();
                    g.DrawPath(Pens.Black, gpath.Path);
                }

                bmp.Save("abcdefg.png");
            }


            Assert.NotNull(res.Path);
        }
    }
}

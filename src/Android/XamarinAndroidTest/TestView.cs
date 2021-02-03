using System;
using System.Reflection;
using Das.Serializer;
using Das.Views;
using Das.Views.Controls;
using Das.Views.Core;
using Das.Views.Core.Enums;
using Das.Views.Images;
using Das.Views.Images.Svg;

namespace XamarinAndroidTest
{
    public sealed class TestView: Das.Views.Panels.View
    {
        private readonly IImageProvider _imageProvider;

        public TestView(IVisualBootstrapper visualBootstrapper,
                        IImageProvider imageProvider)
        : base(visualBootstrapper)
        {
            _imageProvider = imageProvider;
            
            
            var lblPlayersCount = new Label(visualBootstrapper);
            lblPlayersCount.Text = "I am a gut programming guy";
            

            var pbSvg = new PictureFrame(visualBootstrapper)
            {
                VerticalAlignment = VerticalAlignments.Center, 
                HorizontalAlignment = HorizontalAlignments.Center
            };
            var svg = GetEmbeddedImage("XamarinAndroidTest.Resources.cog.svg");
            //svg.Stroke = Color.Red;
            //svg.Fill = SolidColorBrush.Green;
            pbSvg.Image =svg;
            Content = pbSvg;


        }

        private ISvgImage GetEmbeddedImage(String name)
        {
            var srl = new DasSerializer();
            var bldr = new SvgPathBuilder(_imageProvider, srl);

            var asm = GetType().GetTypeInfo().Assembly;

            //var bobsy = asm.GetManifestResourceNames();

            using (var stream = asm.GetManifestResourceStream(name))
            {
                if (stream == null)
                    throw new InvalidOperationException("Missing Resource: " + name);

                var res = srl.FromXml<SvgDocument>(stream);
                var bob = bldr.Parse(res);
                //var bob = SvgPathBuilder.Parse(res.Path.D);
                return bob;
                //return _imageProvider.GetImage(stream) ??
                //       throw new InvalidOperationException("Missing Resource: " + name);
            }
        }
    }
}
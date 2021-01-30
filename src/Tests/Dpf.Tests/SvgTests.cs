using System;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Images.Svg;
using Xunit;

namespace Dpf.Tests
{
    public class SvgTests : TestBase
    {
        [Fact]
        public void DeserializeSvgXml()
        {
            var settings = DasSettings.Default;
            settings.IsPropertyNamesCaseSensitive = false;
            
            var srl = new DasSerializer(settings);

            var xml = GetFileContents("cog.svg");

            //var fullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            //    "Files",
            //    "cog.svg");

            //var fi = new FileInfo(fullName);

            var res = srl.FromXml<SvgDocument>(xml);

            Assert.NotNull(res.Path);
        }
    }
}

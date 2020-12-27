using System;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Construction;
using Das.Views.Styles.Construction;
using Xunit;

namespace Dpf.Tests
{
    public class StyleSelectorTests
    {
        [Fact]
        public void VisualStatePlusChildBefore()
        {
            var css = @".pure-material-switch > input:checked + span::before {
    background-color:pink;
}";
            var styleSheet = _styleInflator.InflateCss(css);

            Assert.Single(styleSheet.Rules);

        }

        private static readonly DasSerializer _serializer = new DasSerializer();

        private static readonly IStyleInflater _styleInflator = new DefaultStyleInflater(
            _serializer.TypeInferrer);
    }
}

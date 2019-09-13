using Das.Gdi;
using System.IO;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Panels;
using Das.Views;

namespace ViewCompiler
{
    public class ViewProvider : GdiProvider, IViewProvider
    {
        public ViewProvider()
        {
            var serializer = GetViewDeserializer();
            _builder = new ViewBuilderProvider(serializer);
        }

        public async Task<IView> GetView(FileInfo file)
        {
            var bldr = await _builder.GetViewBuilder(file);
            return bldr;
        }

        private readonly ViewBuilderProvider _builder;

        
        public static ViewDeserializer GetViewDeserializer()
        {
            var settings = DasSettings.Default;
            settings.TypeSearchNameSpaces = new[]
            {
                "Das.Views.Controls",
                "Das.Views.Panels",
                "TestCommon"
            };
            settings.NotFoundBehavior = TypeNotFound.NullValue;
            var serializer = new ViewDeserializer(settings);
            return serializer;
        }
    }
}

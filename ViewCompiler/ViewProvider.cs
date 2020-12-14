using System;
using Das.Gdi;
using System.IO;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Panels;
using Das.Views;

namespace ViewCompiler
{
    public class ViewProvider : GdiProvider, 
                                IViewProvider
    {
        public ViewProvider()
        {
            var serializer = GetViewDeserializer();
            _builder = new ViewBuilderProvider(serializer);

            TypeInferrer = serializer.TypeInferrer;
            ViewDeserializer = serializer;
        }

        public async Task<IVisualElement> GetView(FileInfo file)
        {
            var bldr = await _builder.GetViewBuilder(file);
            return bldr;
        }

        private readonly ViewBuilderProvider _builder;
        public ITypeInferrer TypeInferrer { get; }
        public ViewDeserializer ViewDeserializer { get; }

        
        protected static ViewDeserializer GetViewDeserializer()
        {
            //var settings = DasSettings.Default;
            //settings.TypeSearchNameSpaces = new[]
            //{
            //    "Das.Views.Controls",
            //    "Das.Views.Panels",
            //    "TestCommon"
            //};
            //settings.NotFoundBehavior = TypeNotFound.NullValue;
            //settings.PropertySearchDepth = TextPropertySearchDepths.AsTypeInNamespacesAndSystem;
            var serializer = new ViewDeserializer();
            return serializer;
        }
    }
}

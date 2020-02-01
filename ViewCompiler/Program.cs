using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Types;

namespace ViewCompiler
{
    static class Program
    {
        [STAThread]
        static void Main(String[] args)
        {
            var files = new List<FileInfo>();
            foreach (var argh in args)
            {
                if (File.Exists(argh))
                    files.Add(new FileInfo(argh));
            }

            var compiling = Compile(files);
            compiling.ConfigureAwait(false);
            compiling.Wait();
        }

        private static async Task Compile(IEnumerable<FileInfo> files)
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
            var viewBuilderProvider = new ViewBuilderProvider(serializer);
            var typeMani = serializer.TypeManipulator;//  new TypeManipulator(settings, new NodePool(settings, new NodeTypeProvider()));
           // var maniPedi = new ObjectManipulator(typeMani);
            var typeBuilder = new ViewTypeBuilder(serializer, serializer.ObjectManipulator,//  maniPedi,
                settings);

            foreach (var file in files)
            {
                var bldr = await viewBuilderProvider.GetViewBuilder(file);

                var viewNameEnd = file.Name.LastIndexOf(file.Extension, StringComparison.OrdinalIgnoreCase);
                var name = file.Name.Substring(0, viewNameEnd);

                var dType = typeBuilder.BuildViewType(bldr, name);
                // ReSharper disable once UnusedVariable
                var lol = Activator.CreateInstance(dType);
            }

            //typeBuilder.Save();
        }
    }
}

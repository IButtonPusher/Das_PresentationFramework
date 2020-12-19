using System.IO;
using System.Threading.Tasks;
using Das.Views.DevKit;
using Das.Views.Styles;

namespace ViewCompiler
{
    public class ViewBuilderProvider
    {
        public ViewBuilderProvider(ViewDeserializer serializer)
        {
            _serializer = serializer;
        }

        private readonly ViewDeserializer _serializer;

        /// <summary>
        /// Turns a file containing valid json markup into a ViewBuilder object
        /// </summary>
        public async Task<ViewBuilder?> GetViewBuilder(FileInfo file)
        {
            ViewBuilder bldr;

            bldr = _serializer.FromJson<ViewBuilder>(file);

            //var json = File.ReadAllText(file.FullName);
            //bldr = _serializer.FromJson<ViewBuilder>(json);


            //using (var stream = file.OpenRead())
            //{
                

            //    //bldr = await _serializer.FromJsonAsync<ViewBuilder>(stream);
            //    bldr = _serializer.FromJson<ViewBuilder>(stream);
            //}

            if (bldr == null)
                return null;

            bldr.Serializer = _serializer;

            var styleContext = new BaseStyleContext(DefaultStyle.Instance,
                new DefaultColorPalette());

            foreach (var style in _serializer.GetStyles())
                styleContext.RegisterStyle(style.Item1, style.Item2);

            _serializer.PostDeserialize();

            bldr.StyleContext = styleContext;

            return await Task.FromResult(bldr);
        }
    }
}

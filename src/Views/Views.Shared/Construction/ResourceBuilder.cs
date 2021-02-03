using System;
using System.Reflection;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Core;
using Das.Views.Images;


namespace Das.Views.Construction
{


    public class ResourceBuilder : IResourceBuilder
    {
        public ResourceBuilder(IImageProvider imageProvider,
                               IXmlSerializer serializer,
                               ISvgPathBuilder svgBuilder)
        {
            _imageProvider = imageProvider;
            _svgBuilder = svgBuilder;
            
        }

        public async Task<Object?> GetEmbeddedResourceAsync(String path,
                                                            String[] pathTokens,
                                                            Assembly assembly)
        {
            using (var stream = assembly.GetManifestResourceStream(path))
            {
                if (stream == null)
                    return default;

                var fileExtension = pathTokens[pathTokens.Length - 1].ToLower();

                switch (fileExtension)
                {
                    case SVG:
                        var svg = await _svgBuilder.LoadAsync(stream);
                        return svg;

                    case PNG:
                    case JPEG:
                    case GIF:
                        return _imageProvider.GetImage(stream);

                    default:
                        return default;
                }
            }
        }

        private const String SVG = "svg";
        private const String PNG = "png";
        private const String JPEG = "jpeg";
        private const String GIF = "gif";
        private readonly IImageProvider _imageProvider;
        private readonly ISvgPathBuilder _svgBuilder;
    }
}

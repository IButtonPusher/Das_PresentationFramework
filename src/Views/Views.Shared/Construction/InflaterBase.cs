using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Das.Views.Construction
{
    public abstract class InflaterBase
    {
        protected static async Task<String> GetStringFromResourceAsync(String resourceName)
        {
            var thisExe = Assembly.GetExecutingAssembly();

            using (var stream = thisExe.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException(resourceName);

                using (var sr = new StreamReader(stream))
                {
                    return await sr.ReadToEndAsync().ConfigureAwait(false);
                }
            }
        }

        protected static String GetStringFromResource(String resourceName)
        {
            var thisExe = Assembly.GetExecutingAssembly();

            using (var stream = thisExe.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException(resourceName);

                using (var sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        protected static IMarkupNode GetRootNode(String xml)
        {
            IMarkupNode? node = XmlNodeBuilder.GetMarkupNode(xml);

            if (node == null)
                throw new InvalidOperationException();

            if (node.IsEncodingHeader)
                node = node[0];

            return node;
        }
        
    }
}

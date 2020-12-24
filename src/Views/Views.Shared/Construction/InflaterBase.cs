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
        
    }
}

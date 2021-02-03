using System;
using System.IO;
using System.Threading.Tasks;

namespace Das.Views.Images
{
    public class NullSvgBuilder : ISvgPathBuilder
    {
        public static readonly NullSvgBuilder Instance = new();

        private NullSvgBuilder(){}

        public Task<ISvgImage?> LoadAsync(Stream stream)
        {
            #if NET40

            return TaskEx.FromResult<ISvgImage?>(default);

            #else
            
            return Task.FromResult<ISvgImage?>(default);

            #endif
        }
    }
}

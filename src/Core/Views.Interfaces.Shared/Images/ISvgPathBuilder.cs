using System;
using System.IO;
using System.Threading.Tasks;

namespace Das.Views.Images
{
    public interface ISvgPathBuilder
    {
        Task<ISvgImage?> LoadAsync(Stream stream);

        ISvgImage? Load(Stream stream);
    }
}

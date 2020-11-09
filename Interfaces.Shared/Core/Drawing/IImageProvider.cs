using System;
using System.IO;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;

namespace Das.Views.Core
{
    public interface IImageProvider
    {
        IImage? GetImage(Stream stream);

        IImage? GetImage(Byte[] bytes);

        IImage GetNullImage();
    }
}
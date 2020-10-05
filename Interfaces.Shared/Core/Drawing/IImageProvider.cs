using System;
using System.IO;
using Das.Views.Core.Drawing;

namespace Das.Views.Core
{
    public interface IImageProvider
    {
        IImage? GetImage(Stream stream);
    }
}

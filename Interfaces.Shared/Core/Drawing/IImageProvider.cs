using System;
using System.IO;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;

namespace Das.Views.Core
{
    public interface IImageProvider
    {
        IImage? GetImage(Stream stream);

        /// <summary>
        ///     Scales the image down based on available client viewing area
        ///     (e.g. window size, mobile screen resolution)
        /// </summary>
        IImage? GetImage(Stream stream,
                         Double maximumWidthPct);

        IImage? GetImage(Byte[] bytes);

        IImage? GetImage(FileInfo file);

        IImage GetNullImage();
    }
}
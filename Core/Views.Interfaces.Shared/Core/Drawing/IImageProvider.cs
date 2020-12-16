using System;
using System.IO;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;

namespace Das.Views.Core
{
    public interface IImageProvider
    {
        IImage? GetImage(Stream stream);

        IImage? GetImage(Stream stream,
                         Boolean isPreserveStream);

        /// <summary>
        ///     Scales the image down (or up) based on available client viewing area
        ///     (e.g. window size, mobile screen resolution)
        /// </summary>
        IImage? GetDeviceScaledImage(Stream stream,
                                     Double maximumWidthPct,
                                     Boolean isPreserveStream);

        IImage GetScaledImage(IImage input,
                              Double width,
                              Double height);

        IImage? GetImage(Byte[] bytes);

        IImage? GetImage(Byte[] bytes,
                         Boolean isPreserveStream);

        IImage? GetImage(FileInfo file);

        IImage GetNullImage();

        Double DeviceEffectiveDpi { get; }
    }
}
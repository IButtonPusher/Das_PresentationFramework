using System;
using System.IO;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Images;

namespace Das.OpenGL
{
    public class GLImageProvider : IImageProvider
    {
        public IImage? GetImage(Stream stream)
        {
            throw new NotImplementedException();
        }

        public IImage? GetImage(Stream stream,
                                Boolean isPreserveStream)
        {
            throw new NotImplementedException();
        }

        public IImage? GetDeviceScaledImage(Stream stream,
                                            Double maximumWidthPct,
                                            Boolean isPreserveStream)
        {
            throw new NotImplementedException();
        }

        public IImage GetScaledImage(IImage input,
                                     Double width,
                                     Double height)
        {
            throw new NotImplementedException();
        }

        public IImage? GetImage(Byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public IImage? GetImage(Byte[] bytes,
                                Boolean isPreserveStream)
        {
            throw new NotImplementedException();
        }

        public IImage? GetImage(FileInfo file)
        {
            throw new NotImplementedException();
        }

        public IImage GetNullImage()
        {
            throw new NotImplementedException();
        }

        public Double DeviceEffectiveDpi => 1.0;

        public IGraphicsPath GetNewGraphicsPath()
        {
            throw new NotImplementedException();
        }

        public IImage GetImage(IGraphicsPath path,
                               IColor foreground)
        {
            throw new NotImplementedException();
        }
    }
}

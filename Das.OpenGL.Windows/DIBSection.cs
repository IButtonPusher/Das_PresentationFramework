using System;
using System.Runtime.InteropServices;
using Das.Views.Winforms;

namespace Das.OpenGL.Windows
{
    public class DIBSection : IDisposable
    {
		public virtual void Create(IntPtr hDC, Int32 width, Int32 height, int bitCount)
        {
            _width = width;
            _height = height;
            _parentDC = hDC;

            //	Destroy existing objects.
            Destroy();

            //	Create a bitmap info structure.
            var info = new BitMapInfo();
            info.Init();

            //	Set the data.
            info.biBitCount = (short)bitCount;
            info.biPlanes = 1;
            info.biWidth = _width;
            info.biHeight = _height;

            //	Create the bitmap.
            _hBitmap = Native.CreateDIBSection(hDC, ref info, 0,
                out _bits, IntPtr.Zero, 0);

            Native.SelectObject(hDC, _hBitmap);

            //	Set the OpenGL pixel format.
            SetPixelFormat(hDC, bitCount);
        }

        /// <summary>
        /// Resizes the section.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="bitCount">The bit count.</param>
        public void Resize(int width, int height, int bitCount)
        {
            //	Destroy existing objects.
            Destroy();

            //  Set parameters.
            Width = width;
            Height = height;

            //	Create a bitmap info structure.
            var info = new BitMapInfo();
            info.Init();

            //	Set the data.
            info.biBitCount = (short)bitCount;
            info.biPlanes = 1;
            info.biWidth = width;
            info.biHeight = height;

            //	Create the bitmap.
            _hBitmap = Native.CreateDIBSection(_parentDC, ref info, 0,
                out _bits, IntPtr.Zero, 0);

            Native.SelectObject(_parentDC, _hBitmap);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Destroy();
        }

        public const byte PFD_TYPE_RGBA = 0;
        public const uint PFD_DRAW_TO_BITMAP = 8;
        public const uint PFD_SUPPORT_GDI = 16;
        public const uint PFD_SUPPORT_OPENGL = 32;
        public const sbyte PFD_MAIN_PLANE = 0;
      
        protected virtual bool SetPixelFormat(IntPtr hDC, int bitCount)
        {
            //	Create the big lame pixel format majoo.
            var pixelFormat = new Pixelformatdescriptor();
            pixelFormat.Init();

            //	Set the values for the pixel format.
            pixelFormat.nVersion = 1;
            pixelFormat.dwFlags = (PFD_DRAW_TO_BITMAP | PFD_SUPPORT_OPENGL | PFD_SUPPORT_GDI);
            pixelFormat.iPixelType = PFD_TYPE_RGBA;
            pixelFormat.cColorBits = (byte)bitCount;
            pixelFormat.cDepthBits = 32;
            pixelFormat.iLayerType = PFD_MAIN_PLANE;

            //	Match an appropriate pixel format 
            int iPixelformat;
            if ((iPixelformat = Native.ChoosePixelFormat(hDC, pixelFormat)) == 0)
                return false;

            //	Sets the pixel format
            if (Native.SetPixelFormat(hDC, iPixelformat, pixelFormat) == 0)
            {
                var _ = Marshal.GetLastWin32Error();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Destroys this instance.
        /// </summary>
        public virtual void Destroy()
        {
            //	Destroy the bitmap.
            if (_hBitmap != IntPtr.Zero)
            {
                Native.DeleteObject(_hBitmap);
                _hBitmap = IntPtr.Zero;
            }
        }

        /// <summary>
        /// The parent dc.
        /// </summary>
        protected IntPtr _parentDC = IntPtr.Zero;

        /// <summary>
        /// The bitmap handle.
        /// </summary>
        protected IntPtr _hBitmap = IntPtr.Zero;

        /// <summary>
        /// The bits.
        /// </summary>
        protected IntPtr _bits = IntPtr.Zero;

        /// <summary>
        /// The width.
        /// </summary>
		protected int _width;

        /// <summary>
        /// The height.
        /// </summary>
        protected int _height;

        /// <summary>
        /// Gets the handle to the bitmap.
        /// </summary>
        /// <value>The handle to the bitmap.</value>
		public IntPtr HBitmap => _hBitmap;

        /// <summary>
        /// Gets the bits.
        /// </summary>
        public IntPtr Bits => _bits;

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
		public int Width
        {
            get => _width;
            protected set => _width = value;
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
		public int Height
        {
            get => _height;
            protected set => _height = value;
        }
    }
}

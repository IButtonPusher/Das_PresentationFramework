using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Windows;

namespace Das.Views.Winforms
{
    /// <summary>
    /// Base type for hosting content in windows forms.  Prevents other painting and propogates load and 
    /// size changed events
    /// </summary>
    public class HostedControl : Control, IWindowsHost
    {
        public HostedControl()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer,
                true);

            AvailableSize = new Core.Geometry.Size(Width, Height);
            _graphicsDeviceContextPromise = new TaskCompletionSource<IntPtr>();
        }

        public Core.Geometry.Size AvailableSize { get; }

        public virtual Boolean IsLoaded => _isLoaded;

        private Boolean _isLoaded;

        public event Action<ISize>? AvailableSizeChanged;

        public event Func<Task>? HostCreated;
       

        public void Invoke(Action action) => base.Invoke(action);

        public Task InvokeAsync(Action action)
        {
            return this.RunInvokeAsync(action);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            OnResizeEnded();
        }

        protected override async void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            OnResizeEnded();
            _isLoaded = true;

            await InvokeAsync(() =>
            {
                _hostGraphics = Graphics.FromHwnd(Handle);
                var hostDc = _hostGraphics.GetHdc();
                _graphicsDeviceContextPromise.TrySetResult(hostDc);
            });

            await HostCreated.InvokeAsyncEvent(true);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {

        }

        public void OnResizeEnded()
        {
            var av = AvailableSize;

            av.Width = Width;
            av.Height = Height;
            AvailableSizeChanged?.Invoke(av);
        }

        public Task<IntPtr> GraphicsDeviceContextPromise => _graphicsDeviceContextPromise.Task;

        private readonly TaskCompletionSource<IntPtr> _graphicsDeviceContextPromise;
        
        /// <summary>
        /// we keep a reference to this guy so he doesn't get GC'd
        /// </summary>
        private Graphics? _hostGraphics;
    }

    public class HostedControl<TAsset> : HostedControl, IVisualHost<TAsset>
    {
        public HostedControl(IVisualRenderer visual)
        {
            Visual = visual;
        }

        public virtual IPoint2D GetOffset(IPoint2D input)
        {
            return Point2D.Empty;
        }

        public IVisualRenderer Visual { get; }

        public TAsset Asset { get; set; }
    }
}

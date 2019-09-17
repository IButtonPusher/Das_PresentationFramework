using System;
using System.Windows.Forms;
using Das.Views.Core.Geometry;

namespace Das.Views.Winforms
{
    /// <summary>
    /// Base type for hosting content.  Prevents other painting and propogates load and 
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

            AvailableSize = new Size(Width, Height);
        }

        public Size AvailableSize { get; }

        public virtual Boolean IsLoaded => _isLoaded;

        private Boolean _isLoaded;

        public event EventHandler AvailableSizeChanged;

        public event EventHandler HostCreated
        {
            add => HandleCreated += value;
            remove => HandleCreated -= value;
        }

        public void Invoke(Action action) => base.Invoke(action);

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            OnResizeEnded();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            OnResizeEnded();
            _isLoaded = true;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {

        }

        public void OnResizeEnded()
        {
            AvailableSize.Width = Width;
            AvailableSize.Height = Height;
            AvailableSizeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

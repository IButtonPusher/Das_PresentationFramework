using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Size = Das.Views.Core.Geometry.Size;

namespace Das.Views.Winforms
{
    public class VisualForm : Form, IVisualHost<Bitmap>
    {
        private readonly HostedControl<Bitmap> _control;

        public VisualForm(IVisualRenderer visual,
                             HostedControl<Bitmap> control)
        {
            _control = control;
            _availableSize = new Size(Width, Height);
            Visual = visual;
        }

        protected override async void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            await HostCreated.InvokeAsyncEvent(true);
        }

        public T Invoke<T>(Func<T> action)
        {
            return this.RunInvoke(action);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (_control == null)
                return;

            _availableSize.Width = _control.Width;
            _availableSize.Height = _control.Height;
            //_isChanged = true;
            AvailableSizeChanged?.Invoke(_availableSize);
        }

        //public Bitmap BackingBitmap
        //{
        //    get => _control.BackingBitmap;
        //    set
        //    {
        //        _contents.BackingBitmap = value;
        //        _isChanged = false;
        //    }
        //}

        public virtual Size AvailableSize => _availableSize;

        public virtual Boolean IsLoaded => _control.IsLoaded;

        public void Invoke(Action action) => base.Invoke(action);

        public Task InvokeAsync(Action action)
        {
            return this.RunInvokeAsync(action);
        }

        public event Func<Task>? HostCreated;

        public event Action<ISize>? AvailableSizeChanged;

        public virtual IPoint2D GetOffset(IPoint2D input) => Point2D.Empty;

        public IVisualRenderer Visual { get; }

        private readonly Size _availableSize;

        public Bitmap Asset
        {
            get => _control.Asset;
            set => _control.Asset = value;
        }
    }
}

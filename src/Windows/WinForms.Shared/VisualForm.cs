using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Size = Das.Views.Core.Geometry.Size;

namespace Das.Views.Winforms
{
   // ReSharper disable once UnusedType.Global
   public class VisualForm : Form, IVisualHost<Bitmap>
    {
        public VisualForm(IVisualRenderer visual,
                          HostedControl<Bitmap> control)
        {
            _control = control;
            _availableSize = new Size(Width, Height);
            Visual = visual;
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

        //public T Invoke<T>(Func<T> action)
        //{
        //    return this.RunInvoke(action);
        //}

        //public void Invoke(Action action)
        //{
        //    base.Invoke(action);
        //}

        //public Task InvokeAsync(Action action)
        //{
        //    return this.RunInvokeAsync(action);
        //}

        public void BeginInvoke(Action action)
        {
            this.RunBeginInvoke(action);
        }

        public void Invoke(Action action) => base.Invoke(action);

        public void Invoke(Action action, 
                           Int32 priority)
        {
            Invoke(action);
        }

        public Task InvokeAsync(Func<Task> action)
        {
           return this.RunInvokeAsync(action);
        }

        public T Invoke<T>(Func<T> action)
        {
            return this.RunInvoke(action);
        }

        public Task InvokeAsync(Action action)
        {
            return this.RunInvokeAsync(action);
        }

        public Task<T> InvokeAsync<T>(Func<T> action)
        {
            return this.RunInvokeAsync(action);
        }

        public Task<TOutput> InvokeAsync<TInput, TOutput>(TInput input,
                                                          Func<TInput, TOutput> action)
        {
            return this.RunInvokeAsync(input, action);
        }

        public Task InvokeAsync<TInput>(TInput input, 
                                        Func<TInput, Task> action)
        {
            return this.RunInvokeAsync(input, action);
        }

        public Task<T> InvokeAsync<T>(Func<Task<T>> action)
        {
            return this.RunInvokeAsync(action);
        }

        public event Func<Task>? HostCreated;

        public event Action<ISize>? AvailableSizeChanged;

        public virtual IPoint2D GetOffset(IPoint2D input)
        {
            return Point2D.Empty;
        }

        public Bitmap Asset
        {
            get => _control.Asset;
            set => _control.Asset = value;
        }

        public IVisualRenderer Visual { get; }

        protected override async void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            await HostCreated.InvokeAsyncEvent(true);
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

        private readonly Size _availableSize;
        private readonly HostedControl<Bitmap> _control;
    }
}
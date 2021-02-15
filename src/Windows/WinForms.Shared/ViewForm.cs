using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Extensions;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Hosting;
using WinForms.Shared;

namespace Das.Views.Winforms
{
    public abstract class ViewForm : Form,
                                     IViewHost
    {
        public ViewForm(HostedViewControl control)
        {
            _availableSize = ValueSize.Empty;
            _contents = control;
            //_availableSize = new ValueSize(Width, Height);

            Controls.Add(_contents);
            _contents.Dock = DockStyle.Fill;
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw,
                true);
        }

        public void BeginInvoke(Action action)
        {
            this.RunBeginInvoke(action);
        }

        public void Invoke(Action action)
        {
            if (!InvokeRequired)
                action();
            else
                base.Invoke(action);
        }

        public void Invoke(Action action,
                           Int32 priority)
        {
            Invoke(action);
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

        public Thickness RenderMargin { get; set; } = Thickness.Empty;

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public SizeToContent SizeToContent { get; set; }

        public abstract IPoint2D GetOffset(IPoint2D input);

        public Boolean IsLoaded => _contents.IsLoaded;

        public event Func<Task>? HostCreated;

        public event Action<ISize>? AvailableSizeChanged;


        public Size AvailableSize => _availableSize;


        public IVisualElement View
        {
            get => _contents.View;
            // ReSharper disable once UnusedMember.Global
            protected set => _contents.SetView(value);
        }

        //public virtual IStyleContext StyleContext => _contents.StyleContext;

        public Double ZoomLevel
        {
            get => _contents.ZoomLevel;
            set => _contents.ZoomLevel = value;
        }

        public void AcceptChanges()
        {
            ((IChangeTracking) _contents).AcceptChanges();
        }

        public Boolean IsChanged => _isHandleCreated &&  _contents.IsChanged;

        protected override Boolean DoubleBuffered
        {
            get => true;
            set { }
        }

        protected override async void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _isHandleCreated = true;

            await HostCreated.InvokeAsyncEvent(true);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SynchronizeSizeToContent();
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            _contents.OnResizeEnded();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (_contents == null)
                return;

            if (!SynchronizeSizeToContent())
                return;

            _contents.View.InvalidateMeasure();
        }

        private Boolean SynchronizeSizeToContent()
        {
            if (_availableSize.Width.AreEqualEnough(_contents.Width) &&
                _availableSize.Height.AreEqualEnough(_contents.Height))
                return false;

            _availableSize = new ValueSize(_contents.Width, _contents.Height);
            AvailableSizeChanged?.Invoke(_availableSize);
            return true;
        }

        private readonly HostedViewControl _contents;
        private ValueSize _availableSize;
        private Boolean _isHandleCreated;

        public IColorPalette ColorPalette => _contents.ColorPalette;
    }
}
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;
using Das.Views.Mvvm;
using WinForms.Shared;
using Size = Das.Views.Core.Geometry.Size;

namespace Das.Views.Winforms
{
    public abstract class ViewForm : Form, 
                                     IViewHost
    {
        public ViewForm(HostedViewControl control) //: base(control.View, control)
        {
            _contents = control;
            _availableSize = new Size(Width, Height);
            _changeLock = new Object();

            Controls.Add(_contents);
            _contents.Dock = DockStyle.Fill;
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw,
                true);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _isChanged = true;
        }

        private readonly HostedViewControl _contents;

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (_contents == null)
                return;

            _availableSize.Width = _contents.Width;
            _availableSize.Height = _contents.Height;
            _isChanged = true;
            AvailableSizeChanged?.Invoke(_availableSize);
        }

        protected override async void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            await HostCreated.InvokeAsyncEvent(true);
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            _contents.OnResizeEnded(); 
        }

        //public T Invoke<T>(Func<T> action)
        //{
        //    return this.RunInvoke(action);
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

        private readonly Size _availableSize;

        protected override Boolean DoubleBuffered
        {
            get => true;
            set { }
        }

        public Thickness RenderMargin { get; set; } = Thickness.Empty;

        public abstract IPoint2D GetOffset(IPoint2D input);

        public Boolean IsLoaded => _contents.IsLoaded;

        public event Func<Task>? HostCreated;

        public event Action<ISize>? AvailableSizeChanged;
        

        //public void Invoke(Action action) => base.Invoke(action);

        //public Task InvokeAsync(Action action)
        //{
        //    return this.RunInvokeAsync(action);
        //}

        private Boolean _isChanged;
        private readonly Object _changeLock;

        public void AcceptChanges()
        {
            lock (_changeLock)
                _isChanged = false;

            _contents.AcceptChanges();
        }

        public virtual Boolean IsChanged
        {
            get
            {
                if (View != null && View.IsChanged)
                    return true;

                lock (_changeLock)
                    return _isChanged;
            }
        }

        public IViewModel? DataContext
        {
            get => _contents.DataContext;
            set => _contents.DataContext = value;
        }

        public Size AvailableSize => _availableSize;
        public T GetStyleSetter<T>(StyleSetter setter, 
                                   IVisualElement element)
            => StyleContext.GetStyleSetter<T>(setter, element);

        public T GetStyleSetter<T>(StyleSetter setter, 
                                   StyleSelector selector, 
                                   IVisualElement element)
            => StyleContext.GetStyleSetter<T>(setter, selector, element);

        public void RegisterStyleSetter(IVisualElement element, StyleSetter setter, Object value)
        {
            _contents.RegisterStyleSetter(element, setter, value);
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetter setter, 
                                        StyleSelector selector, 
                                        Object value)
        {
            StyleContext.RegisterStyleSetter(element, setter, selector, value);
        }

        public IColor GetCurrentAccentColor()
        {
            return _contents.GetCurrentAccentColor();
        }

        public IView View
        {
            get => _contents.View;
            // ReSharper disable once UnusedMember.Global
            protected set => _contents.SetView(value);
        }

        public virtual IStyleContext StyleContext => _contents.StyleContext;

        public Double ZoomLevel
        {
            get => _contents.ZoomLevel;
            set => _contents.ZoomLevel = value;
        }

    }
}

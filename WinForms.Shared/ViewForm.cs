using System;
using System.Windows.Forms;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;
using WinForms.Shared;
using Size = Das.Views.Core.Geometry.Size;

namespace Das.Views.Winforms
{
    public abstract class ViewForm : Form, IViewHost
    {
        public ViewForm(HostedViewControl control)
        {
            _contents = control;
            _availableSize = new Size(Width, Height);

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
            AvailableSizeChanged?.Invoke(this, e);
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            _contents.OnResizeEnded(); 
        }

        private readonly Size _availableSize;

        protected override bool DoubleBuffered
        {
            get => true;
            set { }
        }

        public Thickness RenderMargin { get; set; } = Thickness.Empty;

        public bool IsLoaded => _contents.IsLoaded;

        public event EventHandler HostCreated
        {
            add => HandleCreated += value;
            remove => HandleCreated -= value;
        }

        public event EventHandler AvailableSizeChanged;
        public void Invoke(Action action) => base.Invoke(action);

        private bool _isChanged;

        public void AcceptChanges()
        {

        }

        public virtual bool IsChanged => View != null && View.IsChanged || _isChanged;

        public IViewModel DataContext
        {
            get => _contents.DataContext;
            set => _contents.DataContext = value;
        }

        public Size AvailableSize => _availableSize;
        public T GetStyleSetter<T>(StyleSetters setter, IVisualElement element)
            => StyleContext.GetStyleSetter<T>(setter, element);

        public IView View
        {
            get => _contents.View;
            protected set => _contents.SetView(value);
        }

        public virtual IStyleContext StyleContext => _contents.StyleContext;

        public double ZoomLevel
        {
            get => _contents.ZoomLevel;
            set => _contents.ZoomLevel = value;
        }

    }
}

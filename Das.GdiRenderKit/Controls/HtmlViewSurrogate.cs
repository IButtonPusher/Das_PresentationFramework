using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Views.Controls;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Views.Gdi.Controls
{
    public class HtmlViewSurrogate : WebBrowser, 
                                     IVisualSurrogate 
    {
        private readonly Control _hostingControl;
        private readonly HtmlPanel _htmlPanel;
        private readonly WebBrowser _browser;

        public HtmlViewSurrogate(IVisualElement element,
                                 Control hostingControl)
        {
            if (!(element is HtmlPanel valid))
                throw new InvalidCastException();
            _hostingControl = hostingControl;

            _htmlPanel = valid;
            _browser = this;
            _htmlPanel.PropertyChanged += OnControlPropertyChanged;
        }

        private void OnControlPropertyChanged(Object sender, 
                                              PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(HtmlPanel.Parent):
                    OnParentChanging(_htmlPanel.Parent);
                    break;

                case nameof(HtmlPanel.Markup):
                    DocumentText = _htmlPanel.Markup;
                    break;
            }
        }

        public Type ReplacesType => typeof(HtmlPanel);

        public ISize Measure(ISize availableSpace, 
                             IMeasureContext measureContext)
        {
            return availableSpace;
        }

        public void Arrange(ISize availableSpace, 
                                     IRenderContext renderContext)
        {
            var cube = renderContext.TryGetElementBounds(this);

            Invoke((Action)(() =>
            {
                _browser.Width = Convert.ToInt32(availableSpace.Width);
                _browser.Height = Convert.ToInt32(availableSpace.Height);
                if (cube != null)
                    _browser.Location = new Point(Convert.ToInt32(cube.Left),
                        Convert.ToInt32(cube.Top));
            }));
        }

        public IVisualElement DeepCopy()
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);

            Disposed?.Invoke(this);
        }

        public Int32 Id => -1;

        public new event Action<IVisualElement>? Disposed;

        public void OnParentChanging(IContentContainer? newParent)
        {
            if (newParent == null)
                _hostingControl.Controls.Remove(this);
            else if (!_hostingControl.Controls.Contains(this))
                _hostingControl.Controls.Add(this);
        }
    }
}

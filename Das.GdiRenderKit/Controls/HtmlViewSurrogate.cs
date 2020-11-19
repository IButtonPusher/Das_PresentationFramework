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
        public HtmlViewSurrogate(IVisualElement element,
                                 Control hostingControl)
        {
            if (!(element is HtmlPanel valid))
                throw new InvalidCastException();
            _hostingControl = hostingControl;

            _htmlPanel = valid;
            _browser = this;
            _htmlPanel.PropertyChanged += OnControlPropertyChanged;

            if (valid.Markup != null)
                DocumentText = valid.Markup;
        }

        public Type ReplacesType => typeof(HtmlPanel);

        public ValueSize Measure(IRenderSize availableSpace,
                                 IMeasureContext measureContext)
        {
            return availableSpace.ToValueSize();
        }

        public void InvalidateMeasure()
        {
            _htmlPanel.InvalidateMeasure();
        }

        public void InvalidateArrange()
        {
            _htmlPanel.InvalidateArrange();
        }

        public Boolean IsRequiresMeasure => _htmlPanel.IsRequiresMeasure;

        public Boolean IsRequiresArrange => _htmlPanel.IsRequiresArrange;

        public void Arrange(IRenderSize availableSpace,
                            IRenderContext renderContext)
        {
            var cube = renderContext.TryGetElementBounds(this);

            BeginInvoke((Action) (() =>
            {
                _browser.Width = Convert.ToInt32(availableSpace.Width);
                _browser.Height = Convert.ToInt32(availableSpace.Height);
                if (cube != null)
                    _browser.Location = new Point(Convert.ToInt32(cube.Left),
                        Convert.ToInt32(cube.Top));
            }));
        }

        IVisualElement IDeepCopyable<IVisualElement>.DeepCopy()
        {
            throw new NotSupportedException();
        }

        public Int32 Id => -1;

        public new event Action<IVisualElement>? Disposed;

        public void OnParentChanging(IContainerVisual? newParent)
        {
            if (newParent == null)
                _hostingControl.Controls.Remove(this);
            else if (!_hostingControl.Controls.Contains(this))
                _hostingControl.Controls.Add(this);
        }

        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);

            Disposed?.Invoke(this);
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

                case nameof(HtmlPanel.Uri):
                    if (_htmlPanel.Uri != null)
                        Navigate(_htmlPanel.Uri);
                    break;
            }
        }

        IControlTemplate IVisualElement.Template => throw new NotSupportedException();

        public void AcceptChanges(ChangeType changeType)
        {
            ((IVisualElement) _htmlPanel).AcceptChanges(changeType);
        }

        private readonly WebBrowser _browser;
        private readonly Control _hostingControl;
        private readonly HtmlPanel _htmlPanel;
    }
}
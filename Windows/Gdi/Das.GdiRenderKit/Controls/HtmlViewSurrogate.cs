using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Templates;

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
            _element = element;
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
            var cube = renderContext.TryGetElementBounds(_element);

            BeginInvoke((Action) (() =>
            {
                _browser.Width = Convert.ToInt32(availableSpace.Width);
                _browser.Height = Convert.ToInt32(availableSpace.Height);
                if (cube != null)
                    _browser.Location = new Point(Convert.ToInt32(cube.Left),
                        Convert.ToInt32(cube.Top));
            }));
        }

        

        public Int32 Id => -1;

        public Boolean IsClipsContent
        {
            get => ((IVisualElement) _htmlPanel).IsClipsContent;
            set => ((IVisualElement) _htmlPanel).IsClipsContent = value;
        }

        public new event Action<IVisualElement>? Disposed;

        public void OnParentChanging(IVisualElement? newParent)
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

        IVisualTemplate? ITemplatableVisual.Template => default;

        public void AcceptChanges(ChangeType changeType)
        {
            ((IVisualElement) _htmlPanel).AcceptChanges(changeType);
        }

        public void RaisePropertyChanged(String propertyName, 
                                         Object? value)
        {
            _htmlPanel.RaisePropertyChanged(propertyName, value);
        }

      
        Double? IVisualElement.Width
        {
            get => ((IVisualElement) _htmlPanel).Width;
            set => ((IVisualElement) _htmlPanel).Width = value;
        }

        Double? IVisualElement.Height
        {
            get => _htmlPanel.Height;
            set => _htmlPanel.Height = value;
        }

        public HorizontalAlignments HorizontalAlignment
        {
            get => ((IVisualElement) _htmlPanel).HorizontalAlignment;
            set => ((IVisualElement) _htmlPanel).HorizontalAlignment = value;
        }

        public VerticalAlignments VerticalAlignment
        {
            get => ((IVisualElement) _htmlPanel).VerticalAlignment;
            set => ((IVisualElement) _htmlPanel).VerticalAlignment = value;
        }

        public IBrush? Background
        {
            get => _htmlPanel.Background;
            set => _htmlPanel.Background = value;
        }

        Thickness? IVisualElement.Margin
        {
            get => _htmlPanel.Margin;
            set => _htmlPanel.Margin = value;
        }

        private readonly WebBrowser _browser;
        private readonly IVisualElement _element;
        private readonly Control _hostingControl;
        private readonly HtmlPanel _htmlPanel;

        public Boolean Equals(IVisualElement other)
        {
            return ReferenceEquals(this, other) ||  _htmlPanel.Equals(other);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => _htmlPanel.PropertyChanged += value;
            remove => _htmlPanel.PropertyChanged -= value;
        }

        public IVisualElement ReplacingVisual => _element;
    }
}
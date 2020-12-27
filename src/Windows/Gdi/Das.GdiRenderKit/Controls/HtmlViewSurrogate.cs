using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Views.Controls;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Styles;
using Das.Views.Styles.Declarations;
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
            ReplacingVisual = element;
            _hostingControl = hostingControl;

            _htmlPanel = valid;
            _browser = this;
            _htmlPanel.PropertyChanged += OnControlPropertyChanged;

            if (valid.Markup != null)
                DocumentText = valid.Markup;
        }

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
            var cube = renderContext.TryGetElementBounds(ReplacingVisual);

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

        public String? Class => _htmlPanel.Class;

        public IStyleSheet? Style => _htmlPanel.Style;

        public Boolean IsClipsContent
        {
            get => _htmlPanel.IsClipsContent;
            set => _htmlPanel.IsClipsContent = value;
        }

        public new event Action<IVisualElement>? Disposed;

        public void OnParentChanging(IVisualElement? newParent)
        {
            if (newParent == null)
                _hostingControl.Controls.Remove(this);
            else if (!_hostingControl.Controls.Contains(this))
                _hostingControl.Controls.Add(this);
        }

        IVisualTemplate? ITemplatableVisual.Template
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public void AcceptChanges(ChangeType changeType)
        {
            _htmlPanel.AcceptChanges(changeType);
        }

        public void RaisePropertyChanged(String propertyName,
                                         Object? value)
        {
            _htmlPanel.RaisePropertyChanged(propertyName, value);
        }


        QuantifiedDouble? IVisualElement.Width
        {
            get => _htmlPanel.Width;
            set => _htmlPanel.Width = value;
        }

        QuantifiedDouble? IVisualElement.Height
        {
            get => _htmlPanel.Height;
            set => _htmlPanel.Height = value;
        }

        public QuantifiedDouble? Left
        {
            get => ((IVisualElement) _htmlPanel).Left;
            set => ((IVisualElement) _htmlPanel).Left = value;
        }

        public QuantifiedDouble? Right
        {
            get => ((IVisualElement) _htmlPanel).Right;
            set => ((IVisualElement) _htmlPanel).Right = value;
        }

        public QuantifiedDouble? Top
        {
            get => ((IVisualElement) _htmlPanel).Top;
            set => ((IVisualElement) _htmlPanel).Top = value;
        }

        public QuantifiedDouble? Bottom
        {
            get => ((IVisualElement) _htmlPanel).Bottom;
            set => ((IVisualElement) _htmlPanel).Bottom = value;
        }

        public HorizontalAlignments HorizontalAlignment
        {
            get => _htmlPanel.HorizontalAlignment;
            set => _htmlPanel.HorizontalAlignment = value;
        }

        public VerticalAlignments VerticalAlignment
        {
            get => _htmlPanel.VerticalAlignment;
            set => _htmlPanel.VerticalAlignment = value;
        }

        public IBrush? Background
        {
            get => _htmlPanel.Background;
            set => _htmlPanel.Background = value;
        }

        QuantifiedThickness IVisualElement.Margin
        {
            get => _htmlPanel.Margin;
            set => _htmlPanel.Margin = value;
        }

        //public ISet<String> StyleClasses => _htmlPanel.StyleClasses;

        public Double Opacity => _htmlPanel.Opacity;

        public Visibility Visibility
        {
            get => _htmlPanel.Visibility;
            set => _htmlPanel.Visibility = value;
        }


        public QuantifiedThickness BorderRadius
        {
            get => _htmlPanel.BorderRadius;
            set => _htmlPanel.BorderRadius = value;
        }

        public Boolean IsEnabled
        {
            get => _htmlPanel.IsEnabled;
            set => _htmlPanel.IsEnabled = value;
        }

        public Boolean TryGetDependencyProperty(DeclarationProperty declarationProperty, out IDependencyProperty dependencyProperty)
        {
            return _htmlPanel.TryGetDependencyProperty(declarationProperty, out dependencyProperty);
        }

        public ILabel? BeforeLabel
        {
            get => _htmlPanel.BeforeLabel;
            set => _htmlPanel.BeforeLabel = value;
        }

        public ILabel? AfterLabel
        {
            get => _htmlPanel.AfterLabel;
            set => _htmlPanel.AfterLabel = value;
        }


        public Boolean IsMarkupNameAlias(String markupTag)
        {
            return _htmlPanel.IsMarkupNameAlias(markupTag);
        }

        public Int32 ZIndex => _htmlPanel.ZIndex;

        public Boolean Equals(IVisualElement other)
        {
            return ReferenceEquals(this, other) || _htmlPanel.Equals(other);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => _htmlPanel.PropertyChanged += value;
            remove => _htmlPanel.PropertyChanged -= value;
        }

        public IVisualElement ReplacingVisual { get; }

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

        private readonly WebBrowser _browser;
        private readonly Control _hostingControl;
        private readonly HtmlPanel _htmlPanel;
    }
}
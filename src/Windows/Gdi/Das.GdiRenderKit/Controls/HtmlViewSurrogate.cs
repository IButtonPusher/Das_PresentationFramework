using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Views.BoxModel;
using Das.Views.Controls;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Input;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Styles.Application;
using Das.Views.Styles.Declarations;
using Das.Views.Templates;
using Das.Views.Transforms;

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

        public ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
                                             IMeasureContext measureContext)
            where TRenderSize : IRenderSize
        {
            return availableSpace.ToValueSize();
        }

        public ValueRenderRectangle ArrangedBounds
        {
            get => _htmlPanel.ArrangedBounds;
            set => _htmlPanel.ArrangedBounds = value;
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

        public void Arrange<TRenderSize>(TRenderSize availableSpace,
                                         IRenderContext renderContext)
           where TRenderSize : IRenderSize
        {
           var hadLast = renderContext.TryGetElementBounds(ReplacingVisual, out var cube);

           BeginInvoke((Action) (() =>
           {
              _browser.Width = Convert.ToInt32(availableSpace.Width);
              _browser.Height = Convert.ToInt32(availableSpace.Height);
              if (hadLast)
                 _browser.Location = new Point(Convert.ToInt32(cube.Left),
                    Convert.ToInt32(cube.Top));
           }));
        }


        public Int32 Id => -1;

        public String? Class => _htmlPanel.Class;

        public IAppliedStyle? Style => _htmlPanel.Style;

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
            get => _htmlPanel.Template;
            set => _htmlPanel.Template = value;
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

        QuantifiedDouble? IBoxValue<QuantifiedDouble?>.Left
        {
            get => _htmlPanel.Left;
            //set => _htmlPanel.Left = value;
        }

        QuantifiedDouble? IBoxValue<QuantifiedDouble?>.Right
        {
            get => _htmlPanel.Right;
            //set => _htmlPanel.Right = value;
        }

        QuantifiedDouble? IBoxValue<QuantifiedDouble?>.Top
        {
            get => _htmlPanel.Top;
            //set => _htmlPanel.Top = value;
        }

        QuantifiedDouble? IBoxValue<QuantifiedDouble?>.Bottom
        {
            get => _htmlPanel.Bottom;
            //set => _htmlPanel.Bottom = value;
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

        public IVisualBorder Border
        {
            get => _htmlPanel.Border;
            set => _htmlPanel.Border = value;
        }

        public Boolean IsEnabled
        {
            get => _htmlPanel.IsEnabled;
            set => _htmlPanel.IsEnabled = value;
        }

        TransformationMatrix IVisualElement.Transform
        {
            get => _htmlPanel.Transform;
            set => _htmlPanel.Transform = value;
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


        public Boolean TryHandleInput<TArgs>(TArgs args,
                                             Int32 x,
                                             Int32 y)
           where TArgs : IMouseInputEventArgs<TArgs>
        {
           return _htmlPanel.TryHandleInput(args, x, y);
        }

        public Int32 ZIndex => _htmlPanel.ZIndex;

        public IBoxShadow BoxShadow => _htmlPanel.BoxShadow;

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
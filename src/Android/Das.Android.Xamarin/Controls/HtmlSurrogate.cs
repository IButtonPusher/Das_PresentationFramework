using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Android.Content;
using Android.Views;
using Android.Webkit;
using Das.Views;
using Das.Views.Controls;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Styles.Application;
using Das.Views.Styles.Declarations;
using Das.Views.Templates;
using Das.Views.Transforms;

namespace Das.Xamarin.Android.Controls
{
    public class HtmlSurrogate : WebView,
                                 IVisualSurrogate
    {
        public HtmlSurrogate(HtmlPanel htmlPanel,
                             Context? context,
                             ViewGroup viewGroup,
                             IUiProvider uiProvider)
            : base(context)
        {
            _htmlPanel = htmlPanel;
            _viewGroup = viewGroup;
            _uiProvider = uiProvider;
            _htmlPanel.PropertyChanged += OnControlPropertyChanged;

            _hasPendingContent = htmlPanel.Markup != null || htmlPanel.Uri != null;
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
            if (_hasPendingContent)
            {
                _hasPendingContent = false;

                if (_htmlPanel.Markup != null)
                    LoadData(_htmlPanel.Markup, "text/html; charset=utf-8", "UTF-8");
                else if (_htmlPanel.Uri != null)
                    LoadUrl(_htmlPanel.Uri.AbsoluteUri);
            }

        }

        public event Action<IVisualElement>? Disposed;

        public void AcceptChanges(ChangeType changeType)
        {
            _htmlPanel.AcceptChanges(changeType);
        }

        public void RaisePropertyChanged(String propertyName, Object? value)
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

        QuantifiedDouble? IVisualElement.Left
        {
            get => _htmlPanel.Left;
            set => _htmlPanel.Left = value;
        }

        QuantifiedDouble? IVisualElement.Right
        {
            get => _htmlPanel.Right;
            set => _htmlPanel.Right = value;
        }

        QuantifiedDouble? IVisualElement.Top
        {
            get => _htmlPanel.Top;
            set => _htmlPanel.Top = value;
        }

        QuantifiedDouble? IVisualElement.Bottom
        {
            get => _htmlPanel.Bottom;
            set => _htmlPanel.Bottom = value;
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

        IBrush? IVisualElement.Background
        {
            get => _htmlPanel.Background;
            set => _htmlPanel.Background = value;
        }

        public QuantifiedThickness Margin
        {
            get => _htmlPanel.Margin;
            set => _htmlPanel.Margin = value;
        }

        

        public Double Opacity => _htmlPanel.Opacity;

        Visibility IVisualElement.Visibility
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

        ITransform IVisualElement.Transform
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


        public Boolean IsMarkupNameAlias(String markupTag)
        {
            return _htmlPanel.IsMarkupNameAlias(markupTag);
        }

        public Int32 ZIndex => _htmlPanel.ZIndex;

        Int32 IVisualElement.Id => _htmlPanel.Id;

        String? IVisualElement.Class => _htmlPanel.Class;

        public IAppliedStyle? Style => _htmlPanel.Style;

        public Boolean IsClipsContent
        {
            get => _htmlPanel.IsClipsContent;
            set => _htmlPanel.IsClipsContent = value;
        }

        public void OnParentChanging(IVisualElement? newParent)
        {
            if (newParent == null)
                _viewGroup.RemoveView(this);
            else _viewGroup.AddView(this);
        }

        public Boolean Equals(IVisualElement other)
        {
            return ReferenceEquals(this, other) || _htmlPanel.Equals(other);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => _htmlPanel.PropertyChanged += value;
            remove => _htmlPanel.PropertyChanged -= value;
        }

        public IVisualElement ReplacingVisual => _htmlPanel;

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
                    _uiProvider.Invoke(() =>
                    {
                        LoadData(_htmlPanel.Markup, "text/html; charset=utf-8", "UTF-8");
                    });
                    break;

                case nameof(HtmlPanel.Uri):
                    if (_htmlPanel.Uri != null)
                        _uiProvider.Invoke(() =>
                        {
                            LoadUrl(_htmlPanel.Uri.AbsoluteUri);
                        });
                    break;
            }
        }

        private readonly HtmlPanel _htmlPanel;
        private readonly ViewGroup _viewGroup;
        private readonly IUiProvider _uiProvider;
        private Boolean _hasPendingContent;
        
        IVisualTemplate? ITemplatableVisual.Template
        {
            get => _htmlPanel.Template;
            set => _htmlPanel.Template = value;
        }
    }
}
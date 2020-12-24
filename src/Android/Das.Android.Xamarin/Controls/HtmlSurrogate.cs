using System;
using System.Collections.Generic;
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
using Das.Views.Templates;

namespace Das.Xamarin.Android.Controls
{
    public class HtmlSurrogate : WebView,
                                 IVisualSurrogate
    {
        public HtmlSurrogate(HtmlPanel htmlPanel,
                             Context? context,
                             ViewGroup viewGroup)
            : base(context)
        {
            _htmlPanel = htmlPanel;
            _viewGroup = viewGroup;
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

        IVisualTemplate? ITemplatableVisual.Template => default;

        public void AcceptChanges(ChangeType changeType)
        {
            _htmlPanel.AcceptChanges(changeType);
        }

        public void RaisePropertyChanged(String propertyName, Object? value)
        {
            _htmlPanel.RaisePropertyChanged(propertyName, value);
        }

        Double? IVisualElement.Width
        {
            get => _htmlPanel.Width;
            set => _htmlPanel.Width = value;
        }

        Double? IVisualElement.Height
        {
            get => _htmlPanel.Height;
            set => _htmlPanel.Height = value;
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

        public Thickness? Margin
        {
            get => _htmlPanel.Margin;
            set => _htmlPanel.Margin = value;
        }

        public ISet<String> StyleClasses => _htmlPanel.StyleClasses;

        public Double Opacity => _htmlPanel.Opacity;

        Visibility IVisualElement.Visibility => _htmlPanel.Visibility;

        public Boolean IsEnabled
        {
            get => _htmlPanel.IsEnabled;
            set => _htmlPanel.IsEnabled = value;
        }

        public Boolean IsMarkupNameAlias(String markupTag)
        {
            return _htmlPanel.IsMarkupNameAlias(markupTag);
        }

        public Int32 ZIndex => _htmlPanel.ZIndex;

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

                    LoadData(_htmlPanel.Markup, "text/html; charset=utf-8", "UTF-8");
                    break;

                case nameof(HtmlPanel.Uri):
                    if (_htmlPanel.Uri != null)
                        LoadUrl(_htmlPanel.Uri.AbsoluteUri);
                    break;
            }
        }

        private readonly HtmlPanel _htmlPanel;
        private readonly ViewGroup _viewGroup;
        private Boolean _hasPendingContent;
    }
}
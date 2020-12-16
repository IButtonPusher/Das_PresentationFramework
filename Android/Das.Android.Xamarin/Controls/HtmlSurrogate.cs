using System;
using System.ComponentModel;
using Android.Content;
using Android.Views;
using Android.Webkit;
using Das.Views;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Xamarin.Android.Controls
{
    public class HtmlSurrogate : WebView,
                                  IVisualSurrogate
    {
        private readonly HtmlPanel _htmlPanel;
        private readonly ViewGroup _viewGroup;
        private Boolean _hasPendingContent;

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
            //System.Diagnostics.Debug.WriteLine("measure html surrogate");
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
            //System.Diagnostics.Debug.WriteLine("arrange html surrogate");
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

        public event Action<IVisualElement>? Disposed;

        IControlTemplate IVisualElement.Template => throw new NotSupportedException();

        public void AcceptChanges(ChangeType changeType)
        {
            ((IVisualElement) _htmlPanel).AcceptChanges(changeType);
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
            get => ((IVisualElement) _htmlPanel).HorizontalAlignment;
            set => ((IVisualElement) _htmlPanel).HorizontalAlignment = value;
        }

        public VerticalAlignments VerticalAlignment
        {
            get => ((IVisualElement) _htmlPanel).VerticalAlignment;
            set => ((IVisualElement) _htmlPanel).VerticalAlignment = value;
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

        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);
            Disposed?.Invoke(this);
        }

        public Boolean IsClipsContent
        {
            get => ((IVisualElement) _htmlPanel).IsClipsContent;
            set => ((IVisualElement) _htmlPanel).IsClipsContent = value;
        }

        public void OnParentChanging(IVisualElement? newParent)
        {
            if (newParent == null)
            {
                _viewGroup.RemoveView(this);
            }
            else _viewGroup.AddView(this);
        }

        public Type ReplacesType => typeof(HtmlPanel);

        public Boolean Equals(IVisualElement other)
        {
            return ReferenceEquals(this, other) ||  _htmlPanel.Equals(other);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => _htmlPanel.PropertyChanged += value;
            remove => _htmlPanel.PropertyChanged -= value;
        }
    }
}
using System;
using System.ComponentModel;
using Android.Content;
using Android.Views;
using Android.Webkit;
using Das.Views.Controls;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Xamarin.Android.Controls
{
    public class HtmlSurrogate2 : WebView,
                                  IVisualSurrogate
    {
        private readonly HtmlPanel _htmlPanel;
        private readonly ViewGroup _viewGroup;

        public HtmlSurrogate2(HtmlPanel htmlPanel, 
                              Context? context,
                              ViewGroup viewGroup) 
            : base(context)
        {
            _htmlPanel = htmlPanel;
            _viewGroup = viewGroup;
            _htmlPanel.PropertyChanged += OnControlPropertyChanged;
        }

        public ISize Measure(IRenderSize availableSpace, 
                             IMeasureContext measureContext)
        {
            System.Diagnostics.Debug.WriteLine("measure html surrogate");
            return availableSpace;
        }

        public void Arrange(IRenderSize availableSpace, 
                            IRenderContext renderContext)
        {
         System.Diagnostics.Debug.WriteLine("arrange html surrogate");
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
            }
        }

        public IVisualElement DeepCopy()
        {
            throw new NotImplementedException();
        }

        public event Action<IVisualElement>? Disposed;

        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);
            Disposed?.Invoke(this);
        }

        public void OnParentChanging(IContentContainer? newParent)
        {
            if (newParent == null)
            {
                _viewGroup.RemoveView(this);
            }
            else _viewGroup.AddView(this);
        }

        public Type ReplacesType => typeof(HtmlPanel);
    }
}
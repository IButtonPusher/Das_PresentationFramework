using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Android.Content;
using Android.Views;
using Android.Webkit;
using Das.Views;
using Das.Views.Controls;
using Das.Views.Rendering;

namespace Das.Xamarin.Android.Controls
{
   public class HtmlSurrogate2 : SurrogateView
   {
      public HtmlSurrogate2(Context context,
                            HtmlPanel htmlPanel,
                            WebView nativeView,
                            IUiProvider uiProvider,
                            ViewGroup viewGroup)
         : base(context, htmlPanel, nativeView, viewGroup)
      {
         _uiProvider = uiProvider;
         _htmlPanel = htmlPanel;
         _nativeView = nativeView;
         htmlPanel.PropertyChanged += OnControlPropertyChanged;

         _hasPendingContent = htmlPanel.Markup != null || htmlPanel.Uri != null;
      }

      public override void Arrange<TRenderSize>(TRenderSize availableSpace,
                                                IRenderContext renderContext)
      {
         if (_hasPendingContent)
         {
            _hasPendingContent = false;

            if (_htmlPanel.Markup != null)
               _nativeView.LoadData(_htmlPanel.Markup, "text/html; charset=utf-8", "UTF-8");
            else if (_htmlPanel.Uri != null)
               _nativeView.LoadUrl(_htmlPanel.Uri.AbsoluteUri);
         }
      }

      private void OnControlPropertyChanged(Object sender,
                                            PropertyChangedEventArgs e)
      {
         switch (e.PropertyName)
         {
            case nameof(HtmlPanel.Parent):
               //ReplacingVisual.OnParentChanging(_htmlPanel.Parent);
               OnParentChanging(_htmlPanel.Parent);
               break;

            case nameof(HtmlPanel.Markup):
               _uiProvider.Invoke(() =>
               {
                  _nativeView.LoadData(_htmlPanel.Markup, "text/html; charset=utf-8", "UTF-8");
               });
               break;

            case nameof(HtmlPanel.Uri):
               if (_htmlPanel.Uri != null)
                  _uiProvider.Invoke(() =>
                  {
                     _nativeView.LoadUrl(_htmlPanel.Uri.AbsoluteUri);
                  });
               break;
         }
      }

      private readonly HtmlPanel _htmlPanel;
      private readonly WebView _nativeView;
      private readonly IUiProvider _uiProvider;
      private Boolean _hasPendingContent;
   }
}

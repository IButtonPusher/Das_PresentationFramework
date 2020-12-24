using Android.Views;
using System;
using Das.Views;
using Das.Views.Controls;

namespace Das.Xamarin.Android.Controls
{
    public class AndroidSurrogateProvider
    {
        private readonly IUiProvider _uiProvider;
        private readonly ViewGroup _viewGroup;

        public AndroidSurrogateProvider(IRenderKit renderKit,
                                        IUiProvider uiProvider,
                                        ViewGroup viewGroup)
        {
            _uiProvider = uiProvider;
            _viewGroup = viewGroup;

            renderKit.RegisterSurrogate<HtmlPanel>(GetHtmlPanelSurrogate);
        }
        
        private IVisualSurrogate GetHtmlPanelSurrogate(IVisualElement element)
        {
            return _uiProvider.Invoke(() =>
            {
                if (!(element is HtmlPanel pnl))
                    throw new InvalidOperationException();

                var surrogate = new HtmlSurrogate(pnl,_viewGroup.Context, _viewGroup);

                _viewGroup.AddView(surrogate);

                return surrogate;
            });
        }
        
    }
}
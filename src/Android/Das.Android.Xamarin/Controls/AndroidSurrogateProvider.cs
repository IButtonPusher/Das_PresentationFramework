﻿using System;
using System.Threading.Tasks;
using Android.Views;
using Das.Views;
using Das.Views.Controls;

namespace Das.Xamarin.Android.Controls
{
    public class AndroidSurrogateProvider
    {
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

                //var webView = new WebView(_viewGroup.Context);
                //var surrogate = new HtmlSurrogate2(_viewGroup.Context, pnl, webView, _uiProvider, _viewGroup);
                
                var surrogate = new HtmlSurrogate(pnl, _viewGroup.Context, _viewGroup, _uiProvider);

                _viewGroup.AddView(surrogate);

                return surrogate;
            });
        }

        private readonly IUiProvider _uiProvider;
        private readonly ViewGroup _viewGroup;
    }
}

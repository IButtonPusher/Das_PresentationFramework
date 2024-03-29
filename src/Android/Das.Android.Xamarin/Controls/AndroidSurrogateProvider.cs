﻿using System;
using System.Threading.Tasks;
using Android.Views;
using Das.Views;
using Das.Views.Controls;

namespace Das.Xamarin.Android.Controls;

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
         if (element is not HtmlPanel pnl)
            throw new InvalidCastException($"{element} is not of type {typeof(HtmlPanel)}");

         if (_viewGroup.Context is not { } viewGroupContext)
            throw new InvalidOperationException($"Null context for {_viewGroup}");

         var surrogate = new HtmlSurrogate(pnl, viewGroupContext, _viewGroup, _uiProvider);

         _viewGroup.AddView(surrogate);

         return surrogate;
      });
   }

   private readonly IUiProvider _uiProvider;
   private readonly ViewGroup _viewGroup;
}

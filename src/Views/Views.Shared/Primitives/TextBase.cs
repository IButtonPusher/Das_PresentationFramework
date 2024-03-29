﻿using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Writing;
using Das.Views.DataBinding;
using Das.Views.DependencyProperties;

namespace Das.Views.Primitives
{
   public abstract class TextBase : BindableElement,
                                    ITextVisual
   {
      protected TextBase(IVisualBootstrapper visualBootstrapper) : base(visualBootstrapper)
      {
         _meText = this;
         _meFont = this;
      }

      public String Text
      {
         get => TextProperty.GetValue(_meText);
         set => TextProperty.SetValue(this, value);
      }

      public IBrush? TextBrush
      {
         get => TextBrushProperty.GetValue(_meText);
         set => TextBrushProperty.SetValue(this, value);
      }

      public FontStyle FontWeight
      {
         get => FontWeightProperty.GetValue(_meFont);
         set => FontWeightProperty.SetValue(this, value);
      }

      public String FontName
      {
         get => FontNameProperty.GetValue(_meFont);
         set => FontNameProperty.SetValue(this, value);
      }

      public Double FontSize
      {
         get => FontSizeProperty.GetValue(_meFont);
         set => FontSizeProperty.SetValue(this, value);
      }

      protected virtual void OnFontChanged()
      {
         _font = default;
      }

      private static void OnFontWeightChanged(IFontVisual visual,
                                              FontStyle arg2,
                                              FontStyle arg3)
      {
         if (visual is TextBase tb)
            tb.OnFontChanged();
      }

      private static void OnFontNameChanged(IFontVisual visual,
                                            String arg2,
                                            String arg3)
      {
         if (visual is TextBase tb)
            tb.OnFontChanged();
      }

      private static void OnFontSizeChanged(IFontVisual visual,
                                            Double arg2,
                                            Double arg3)
      {
         if (visual is TextBase tb)
            tb.OnFontChanged();
      }

      protected Font Font
      {
         get
         {
            if (_font != null)
               return _font;

            _font = new Font(FontSize, FontName, FontWeight);

            return _font;
         }
      }

      private readonly ITextVisual _meText;
      private readonly IFontVisual _meFont;


      public static readonly DependencyProperty<ITextVisual, String> TextProperty =
         DependencyProperty<ITextVisual, String>.Register(nameof(Text), String.Empty,
            PropertyMetadata.AffectsMeasure);

      public static readonly DependencyProperty<ITextVisual, IBrush?> TextBrushProperty =
         DependencyProperty<ITextVisual, IBrush?>.Register(nameof(TextBrush), default);

      public static readonly DependencyProperty<IFontVisual, FontStyle> FontWeightProperty =
         DependencyProperty<IFontVisual, FontStyle>.Register(
            nameof(FontWeight), FontStyle.Regular, OnFontWeightChanged);

      public static readonly DependencyProperty<IFontVisual, String> FontNameProperty =
         DependencyProperty<IFontVisual, String>.Register(
            nameof(FontName), "Segoe UI", OnFontNameChanged);

      public static readonly DependencyProperty<IFontVisual, Double> FontSizeProperty =
         DependencyProperty<IFontVisual, Double>.Register(
            nameof(FontSize), 10, OnFontSizeChanged);

      protected Font? _font;
   }
}

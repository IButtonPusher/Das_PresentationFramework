using System;
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
        }

        public String Text
        {
            get => TextProperty.GetValue(this);
            set => TextProperty.SetValue(this, value);
        }

        public IBrush? TextBrush
        {
            get => TextBrushProperty.GetValue(this);
            set => TextBrushProperty.SetValue(this, value);
        }


        public static readonly DependencyProperty<ITextVisual, String> TextProperty =
            DependencyProperty<ITextVisual, String>.Register(nameof(Text), String.Empty,
                PropertyMetadata.AffectsMeasure);

        public static readonly DependencyProperty<ITextVisual, IBrush?> TextBrushProperty =
            DependencyProperty<ITextVisual, IBrush?>.Register(nameof(TextBrush), default);

        public static readonly DependencyProperty<ITextVisual, FontStyle> FontWeightProperty =
            DependencyProperty<ITextVisual, FontStyle>.Register(
                nameof(FontWeight), FontStyle.Regular, OnFontWeightChanged);

        private static void OnFontWeightChanged(ITextVisual visual,
                                                FontStyle arg2,
                                                FontStyle arg3)
        {
            if (visual is TextBase tb)
                tb.OnFontChanged();
        }

        protected virtual void OnFontChanged()
        {
            _font = default;
        }
        
        public FontStyle FontWeight
        {
            get => FontWeightProperty.GetValue(this);
            set => FontWeightProperty.SetValue(this, value);
        }

        public static readonly DependencyProperty<ITextVisual, String> FontNameProperty =
            DependencyProperty<ITextVisual, String>.Register(
                nameof(FontName), "Segoe UI", OnFontNameChanged);

        private static void OnFontNameChanged(ITextVisual visual,
                                              String arg2,
                                              String arg3)
        {
            if (visual is TextBase tb)
                tb.OnFontChanged();
        }

        public String FontName
        {
            get => FontNameProperty.GetValue(this);
            set => FontNameProperty.SetValue(this, value);
        }

        public static readonly DependencyProperty<ITextVisual, Double> FontSizeProperty =
            DependencyProperty<ITextVisual, Double>.Register(
                nameof(FontSize), 10, OnFontSizeChanged);

        private static void OnFontSizeChanged(ITextVisual visual,
                                              Double arg2,
                                              Double arg3)
        {
            if (visual is TextBase tb)
                tb.OnFontChanged();
        }

        public Double FontSize
        {
            get => FontSizeProperty.GetValue(this);
            set => FontSizeProperty.SetValue(this, value);
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

        protected Font? _font;
    }
}

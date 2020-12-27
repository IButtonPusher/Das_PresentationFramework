using System;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.DataBinding;
using Das.Views.Rendering;
using Das.Views.Styles;
using Das.Views.Styles.Declarations;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;

#endif

namespace Das.Views.Controls
{
    [ContentProperty(nameof(Text))]
    public class Label : BindableElement,
                         ILabel
    {
        public Label(IVisualBootstrapper visualBootstrapper)
            : base(visualBootstrapper)
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


        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
            var font = GetFont(renderContext);

            var brush = TextBrush ??
                        renderContext.GetStyleSetter<SolidColorBrush>(StyleSetterType.Foreground, this);
            renderContext.DrawString(Text, font, brush, Point2D.Empty);
        }


        public override ValueSize Measure(IRenderSize availableSpace,
                                          IMeasureContext measureContext)
        {
            var font = GetFont(measureContext);
            var size = measureContext.MeasureString(Text, font);
            return size;
        }


        public override String ToString()
        {
            return !String.IsNullOrEmpty(Text) 
                ? Text
                : "Label";
        }

        private Font GetFont(IStyleProvider context)
        {
            if (_font != null)
                return _font;

            var font = context.GetStyleSetter<Font>(StyleSetterType.Font, this);
            var useSize = context.GetStyleSetter<Double>(StyleSetterType.FontSize, this);
            if (!useSize.AreEqualEnough(font.Size))
                font = new Font(useSize, font.FamilyName, font.FontStyle);

            _font = font;
            return _font;
        }

        private static void OnTextChanged(Label sender,
                                          String oldValue, String newValue)
        {
            sender.InvalidateMeasure();
        }

        public override Boolean TryGetDependencyProperty(DeclarationProperty declarationProperty, 
                                                         out IDependencyProperty property)
        {
            switch (declarationProperty)
            {
                case DeclarationProperty.Color:
                    property = TextBrushProperty;
                    return true;
                
                default:
                    return base.TryGetDependencyProperty(declarationProperty, out property);
            }
            
            
        }

        public static readonly DependencyProperty<Label, String> TextProperty =
            DependencyProperty<Label, String>.Register(nameof(Text), String.Empty,
                OnTextChanged);

        public static readonly DependencyProperty<Label, IBrush?> TextBrushProperty =
            DependencyProperty<Label, IBrush?>.Register(nameof(TextBrush), default);


        private Font? _font;
    }
}
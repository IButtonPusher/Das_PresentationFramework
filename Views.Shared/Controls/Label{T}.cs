using System;
using Das.Extensions;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.DataBinding;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Controls
{
    public class Label<TDataContext> : BindableElement<TDataContext>,
                            ILabel
    {
        

        public Label(IVisualBootstrapper visualBootstrapper) : base(visualBootstrapper)
        {
            
        }
        
        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
            var font = GetFont(renderContext);

            var brush = TextBrush ?? 
                        renderContext.GetStyleSetter<SolidColorBrush>(StyleSetter.Foreground, this);
            renderContext.DrawString(Text, font, brush, Point2D.Empty);
        }

        //public override void Dispose()
        //{
        //    //_isDisposed = true;

        //    //if (Interlocked.Add(ref _instanceCount, -1) % 10 == 0)
        //    //    Debug.WriteLine("active labels: " + _instanceCount);
        //    base.Dispose();
        //}

        public override ValueSize Measure(IRenderSize availableSpace,
                                          IMeasureContext measureContext)
        {
            var font = GetFont(measureContext);

            //if (Binding is { } binding)
            //    _currentValue = binding.GetValue(DataContext)?.ToString() ?? String.Empty;
            //else
            //    _currentValue = String.Empty;

            //_currentValue = Binding?.GetValue(DataContext)?.ToString() ?? String.Empty;
            var size = measureContext.MeasureString(Text, font);
            return size;
        }

        private Font GetFont(IStyleProvider context)
        {
            if (_font != null)
                return _font;

            var font = context.GetStyleSetter<Font>(StyleSetter.Font, this);
            var useSize = context.GetStyleSetter<Double>(StyleSetter.FontSize, this);
            if (!useSize.AreEqualEnough(font.Size))
                font = new Font(useSize, font.FamilyName, font.FontStyle);

            _font = font;
            return _font;
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
        
        public override String ToString()
        {
            return "Label: " + Text;
        }

        public static readonly DependencyProperty<ILabel, String> TextProperty =
            DependencyProperty<ILabel, String>.Register(nameof(Text), String.Empty, 
                OnTextChanged);

        private static void OnTextChanged(ILabel sender, 
                                          String oldValue, String newValue)
        {
            sender.InvalidateMeasure();
        }

        public static readonly DependencyProperty<ILabel, IBrush?> TextBrushProperty =
            DependencyProperty<ILabel, IBrush?>.Register(nameof(TextBrush), default);

        //private String _currentValue;
        private Font? _font;
        
        //public override Object? DataContext { get; set; }

        //Object? IBindable.DataContext
        //{
        //    get => DataContext;
        //    set => DataContext = value;
        //}

        //public new T DataContext
        //{
        //    get
        //    {
        //        switch (base.DataContext)
        //        {
        //            case null:
        //            return default!;
                    
        //            case T good:
        //                return good;
                    
        //            default:
        //                throw new InvalidCastException();
        //        }
        //    }
        //    set => base.DataContext = value;
        //}

        //public Boolean Equals(IBindableElement<T> other)
        //{
        //    return ReferenceEquals(this, other);
        //}
    }
}

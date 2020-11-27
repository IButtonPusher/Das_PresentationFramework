using System;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.DataBinding;
using Das.Views.Rendering;
using Das.Views.Styles;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;

#endif

namespace Das.Views.Controls
{
    // ReSharper disable once UnusedType.Global
    public class Label : Label<String>
    {
        public Label(IDataBinding<String> binding,
                     IVisualBootStrapper visualBootStrapper) : base(binding, visualBootStrapper)
        {
        }

        // ReSharper disable once UnusedMember.Global
        public Label(IVisualBootStrapper visualBootStrapper) : base(visualBootStrapper)
        {
        }
    }

    public class Label<T> : BindableElement<T>
    {
        public Label(IVisualBootStrapper visualBootStrapper)
            : base(visualBootStrapper)
        {
            _currentValue = String.Empty;
        }

        public Label(IDataBinding<T> binding,
                     IVisualBootStrapper visualBootStrapper)
            : base(binding, visualBootStrapper)
        {
            _currentValue = String.Empty;

        }


        public String Text
        {
            get => TextProperty.GetValue(this);
            set => TextProperty.SetValue(this, value);
        }

        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
            var font = renderContext.GetStyleSetter<Font>(StyleSetter.Font, this)
                       * renderContext.GetZoomLevel();

            var useSize = renderContext.GetStyleSetter<Double>(StyleSetter.FontSize, this);
            if (!useSize.AreEqualEnough(font.Size))
                font = new Font(useSize, font.FamilyName, font.FontStyle);

            var brush = renderContext.GetStyleSetter<SolidColorBrush>(StyleSetter.Foreground, this);
            renderContext.DrawString(_currentValue, font, brush, Point2D.Empty);
        }

        public override void Dispose()
        {
        }

        public override ValueSize Measure(IRenderSize availableSpace,
                                          IMeasureContext measureContext)
        {
            var font = measureContext.GetStyleSetter<Font>(StyleSetter.Font, this) *
                       measureContext.GetZoomLevel();

            if (Binding is { } binding)
                _currentValue = binding.GetValue(DataContext)?.ToString() ?? String.Empty;
            else
                _currentValue = String.Empty;

            //_currentValue = Binding?.GetValue(DataContext)?.ToString() ?? String.Empty;
            var size = measureContext.MeasureString(_currentValue, font);
            return size;
        }

        public override void SetBoundValue(Object? value)
        {
            DataContext = value;
            if (Binding == null && value is T val)
                Binding = new ObjectBinding<T>(val);
        }

        public override Task SetBoundValueAsync(Object? value)
        {
            SetBoundValue(value);
            return TaskEx.CompletedTask;
        }

        public override String ToString()
        {
            return "Label: " + _currentValue;
        }

        public static readonly DependencyProperty<Label<T>, String> TextProperty =
            DependencyProperty<Label<T>, String>.Register(nameof(Text), String.Empty);

        private String _currentValue;
    }
}
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
        public Label(IDataBinding<String> binding) : base(binding)
        {
        }

        // ReSharper disable once UnusedMember.Global
        public Label()
        {
        }
    }

    public class Label<T> : BindableElement<T>
    {
        public Label()
        {
            _currentValue = String.Empty;
        }

        public Label(IDataBinding<T> binding) : base(binding)
        {
            _currentValue = String.Empty;
        }

        public override void Arrange(ISize availableSpace,
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

        public override ISize Measure(ISize availableSpace,
                                      IMeasureContext measureContext)
        {
            var font = measureContext.GetStyleSetter<Font>(StyleSetter.Font, this) *
                       measureContext.GetZoomLevel();

            if (Binding is {} binding)
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

        private String _currentValue;
    }
}
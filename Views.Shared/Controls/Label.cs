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
        private String _currentValue;

        public Label()
        {
        }

        public Label(IDataBinding<T> binding) : base(binding)
        {
        }

        public override ISize Measure(ISize availableSpace,
            IMeasureContext measureContext)
        {
            var font = measureContext.GetStyleSetter<Font>(StyleSetters.Font, this) *
                       measureContext.ViewState.ZoomLevel;

            _currentValue = Binding.GetValue(DataContext).ToString();
            var size = measureContext.MeasureString(_currentValue, font);
            return size;
        }

        public override void Arrange(ISize availableSpace, IRenderContext renderContext)
        {
            var font = renderContext.GetStyleSetter<Font>(StyleSetters.Font, this)
                       * renderContext.ViewState.ZoomLevel;

            var useSize = renderContext.GetStyleSetter<Double>(StyleSetters.FontSize, this);
            if (!useSize.AreEqualEnough(font.Size))
            {
                font = new Font(useSize, font.FamilyName, font.FontStyle);
            }

            var brush = renderContext.GetStyleSetter<Brush>(StyleSetters.Foreground, this);
            renderContext.DrawString(_currentValue, font, brush, Point.Empty);
        }

        public override void SetBoundValue(Object value)
        {
            DataContext = value;
            if (Binding == null && value is T val)
                Binding = new ObjectBinding<T>(val);
        }

        public override String ToString() => "Label: " + _currentValue;
    }
}
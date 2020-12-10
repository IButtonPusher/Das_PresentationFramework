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
                     IVisualBootstrapper visualBootstrapper) : base(binding, visualBootstrapper)
        {
        }

        // ReSharper disable once UnusedMember.Global
        public Label(IVisualBootstrapper visualBootstrapper) : base(visualBootstrapper)
        {
        }
    }

    public class Label<T> : BindableElement<T>
    {
        //private Int32 _instanceCount;
        //private Boolean _isDisposed;

        public Label(IVisualBootstrapper visualBootstrapper)
            : base(visualBootstrapper)
        {
            //_currentValue = String.Empty;

            //Interlocked.Add(ref _instanceCount, 1);
        }

        public Label(IDataBinding<T> binding,
                     IVisualBootstrapper visualBootstrapper)
            : base(binding, visualBootstrapper)
        {
            //_currentValue = String.Empty;

            //Interlocked.Add(ref _instanceCount, 1);
        }

        public override IVisualElement DeepCopy()
        {
            var res = (Label<T>)base.DeepCopy();
            res.TextBrush = TextBrush;

            return res;
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

        protected override void OnBindingChanged(IDataBinding<T>? obj)
        {
            base.OnBindingChanged(obj);

            if (obj is { } binding)
                Text = binding.GetValue(DataContext)?.ToString() ?? String.Empty;
            else
                Text = String.Empty;
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

        protected override void RefreshBoundValues(Object? dataContext)
        {
            base.RefreshBoundValues(dataContext);
            
            if (BoundValue is String {} str)
                Text = str;
        }

        public override String ToString()
        {
            return "Label: " + Text;
        }

        public static readonly DependencyProperty<Label<T>, String> TextProperty =
            DependencyProperty<Label<T>, String>.Register(nameof(Text), String.Empty, 
                OnTextChanged);

        private static void OnTextChanged(Label<T> sender, 
                                          String oldValue, String newValue)
        {
            sender.InvalidateMeasure();
        }

        public static readonly DependencyProperty<Label<T>, IBrush?> TextBrushProperty =
            DependencyProperty<Label<T>, IBrush?>.Register(nameof(TextBrush), default);

        //private String _currentValue;
        private Font? _font;
    }
}
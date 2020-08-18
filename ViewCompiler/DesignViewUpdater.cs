using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Rendering;
using Das.Views.Styles;
using Brush = Das.Views.Core.Drawing.Brush;
using Font = Das.Views.Core.Writing.Font;
using FontStyle = Das.Views.Core.Writing.FontStyle;
using Rectangle = Das.Views.Core.Geometry.Rectangle;

namespace ViewCompiler
{
    public class DesignViewUpdater : LoopViewUpdater<Bitmap>
    {
        public DesignViewUpdater(IViewHost<Bitmap> viewHost,
                                 IRenderer<Bitmap> renderer,
                                 IMeasureContext measureContext,
                                 IRenderContext renderContext,
                                 Int32 maxFramesPerSecond = 60)
            : base(viewHost, renderer, maxFramesPerSecond)
        {
            MeasureContext = measureContext;
            RenderContext = renderContext;
            _viewHost = viewHost;
            _sbSelected = new StringBuilder();
            _font = new Font(10, "Segoe UI", FontStyle.Regular);
            renderer.Rendering += OnRendering;
        }

        protected override Boolean IsChanged => base.IsChanged || _isChanged;

        private void OnRendering(Object sender, EventArgs e)
        {
            _sbSelected.Clear();
            _isChanged = false;

            var element = _viewHost.View;

            MeasureContext.ViewState = _viewHost;
            var measured = MeasureContext.MeasureElement(element, _viewHost.RenderMargin);
            var selectedVisual = SelectedVisuals?.FirstOrDefault();
            if (selectedVisual == null)
                return;

            _sbSelected.AppendLine(element.ToString());
            _sbSelected.AppendLine("Measured: " + measured);
            _sbSelected.AppendLine("Arranged: " + selectedVisual.Position);

            var nonDefaults = GetNonDefaultSetters(element).ToArray();

            foreach (var kvp in nonDefaults)
                _sbSelected.AppendLine(kvp.Key + ": " + kvp.Value);
            

            _rightRectangle = new Rectangle(_viewHost.AvailableSize.Width -
                                                      _viewHost.RenderMargin.Right,
                0, _viewHost.RenderMargin.Width, _viewHost.AvailableSize.Height);
            RenderContext.ViewState = _viewHost;
            RenderContext.FillRect(_rightRectangle, Brush.DarkGray);

            if (selectedVisual.Element == null)
                return;


            RenderContext.DrawString(_sbSelected.ToString(), _font, Brush.White, _rightRectangle);
        }

        public IMeasureContext MeasureContext { get; }
        public IRenderContext RenderContext { get; }

        private IRenderedVisual[] _selectedVisuals;
        public IRenderedVisual[] SelectedVisuals
        {
            get => _selectedVisuals;
            set
            {
                _selectedVisuals = value;
                _isChanged = true;
            }
        }

        private readonly IViewHost<Bitmap> _viewHost;
        private readonly StringBuilder _sbSelected;
        private IRectangle _rightRectangle;
        private readonly IFont _font;
        private Boolean _isChanged;

        protected IEnumerable<KeyValuePair<StyleSetters, Object>> GetNonDefaultSetters(
            IVisualElement element)
        {
            var set = new HashSet<StyleSetters>();
            var arr = _viewHost.StyleContext.GetStylesForElement(element).ToArray();

            for (var c = 0; c < arr.Length - 1; c++)
            {
                var style = arr[c];

                foreach (var setter in style)
                {
                    if (!set.Add(setter.Key))
                        continue;

                    yield return setter;
                }
            }
        }
    }
}

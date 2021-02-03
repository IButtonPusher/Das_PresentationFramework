using System;
using Das.Views.BoxModel;
using Das.Views.Controls;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;

namespace Das.Views.Defaults
{
    public class DefaultTabItemTemplate : DefaultContentTemplate
    {

        public DefaultTabItemTemplate(IVisualBootstrapper visualBootstrapper)
            : base(visualBootstrapper)
        {
        }

        public override IVisualElement BuildVisual(Object? dataContext)
        {
            var btn = _visualBootstrapper.Instantiate<ToggleButton>();
            
            _visualBootstrapper.ApplyVisualStyling(btn);
            btn.FontSize = 15;
            btn.Background = default;
            btn.DataContext = dataContext;
            btn.Padding = new QuantifiedThickness(5, 10, 5, 10);
            btn.Border = VisualBorder.Empty;

            btn.VerticalAlignment = VerticalAlignments.Default;
            btn.HorizontalAlignment = HorizontalAlignments.Default;

            return btn;
        }
    }
}

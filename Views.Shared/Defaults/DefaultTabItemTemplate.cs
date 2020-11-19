using System;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Defaults
{
    public class DefaultTabItemTemplate : DefaultContentTemplate
    {
        private readonly IVisualBootStrapper _templateResolver;
        

        public DefaultTabItemTemplate(IVisualBootStrapper templateResolver) 
            : base(templateResolver, null)
        {
            _templateResolver = templateResolver;
        }

        public override IVisualElement BuildVisual(Object? dataContext)
        {
            var btn = new ToggleButton<Object?>(_templateResolver)
            {
                DataContext = dataContext
            };
            _templateResolver.StyleContext.RegisterStyleSetter(btn, StyleSetter.BorderBrush,
                SolidColorBrush.Tranparent);
            _templateResolver.StyleContext.RegisterStyleSetter(btn, StyleSetter.Background,
                StyleSelector.Active, SolidColorBrush.Tranparent);
            _templateResolver.StyleContext.RegisterStyleSetter(btn, StyleSetter.Background,
                StyleSelector.Checked, SolidColorBrush.Tranparent);
            _templateResolver.StyleContext.RegisterStyleSetter(btn, StyleSetter.Padding,
                StyleSelector.None, new Thickness(5, 5, 5, 15));
            return btn;
        }
    }
}

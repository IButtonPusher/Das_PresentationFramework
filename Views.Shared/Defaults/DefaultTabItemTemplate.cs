using System;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Defaults
{
    public class DefaultTabItemTemplate : DefaultContentTemplate
    {
        private readonly IVisualBootStrapper _visualBootStrapper;
        

        public DefaultTabItemTemplate(IVisualBootStrapper visualBootStrapper) 
            : base(visualBootStrapper, null)
        {
            _visualBootStrapper = visualBootStrapper;
        }

        public override IVisualElement BuildVisual(Object? dataContext)
        {
            var btn = new ToggleButton<Object?>(_visualBootStrapper)
            {
                DataContext = dataContext
            };
            _visualBootStrapper.StyleContext.RegisterStyleSetter(btn, StyleSetter.BorderBrush,
                SolidColorBrush.Tranparent);
            _visualBootStrapper.StyleContext.RegisterStyleSetter(btn, StyleSetter.Background,
                StyleSelector.Active, SolidColorBrush.Tranparent);
            _visualBootStrapper.StyleContext.RegisterStyleSetter(btn, StyleSetter.Background,
                StyleSelector.Checked, SolidColorBrush.Tranparent);


            _visualBootStrapper.StyleContext.RegisterStyleSetter(btn, StyleSetter.Background,
                StyleSelector.None, SolidColorBrush.White);

            btn.Width = 150;
            btn.Height = 80;
            //_visualBootStrapper.StyleContext.RegisterStyleSetter(btn, StyleSetter.Width,
            //    StyleSelector.None, 150);
            //_visualBootStrapper.StyleContext.RegisterStyleSetter(btn, StyleSetter.Padding,
            //    StyleSelector.None, new Thickness(15, 5, 15, 15));
            _visualBootStrapper.StyleContext.RegisterStyleSetter(btn, StyleSetter.FontSize,
                StyleSelector.None, 15);
            return btn;
        }
    }
}

using System;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.ItemsControls;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Defaults
{
    public class DefaultTabItemTemplate<T> : DefaultContentTemplate<T>
    {
        private readonly ITabControl<T> _tabControl;


        public DefaultTabItemTemplate(IVisualBootstrapper visualBootstrapper,
                                      ITabControl<T> tabControl) 
            : base(visualBootstrapper, null)
        {
            _tabControl = tabControl;
        }

       

        public override IVisualElement BuildVisual(Object? dataContext)
        {
            var btn = new ToggleButton<T>(_visualBootstrapper)
            {
                DataContext = dataContext
            };
            _visualBootstrapper.StyleContext.RegisterStyleSetter(btn, StyleSetter.BorderBrush,
                SolidColorBrush.Tranparent);
            _visualBootstrapper.StyleContext.RegisterStyleSetter(btn, StyleSetter.Background,
                StyleSelector.Active, SolidColorBrush.Tranparent);
            _visualBootstrapper.StyleContext.RegisterStyleSetter(btn, StyleSetter.Background,
                StyleSelector.Checked, SolidColorBrush.Tranparent);


            _visualBootstrapper.StyleContext.RegisterStyleSetter(btn, StyleSetter.Background,
                StyleSelector.None, SolidColorBrush.White);

            //btn.Width = 80;
            //btn.Height = 50;
            //_visualBootStrapper.StyleContext.RegisterStyleSetter(btn, StyleSetter.Width,
            //    StyleSelector.None, 150);
            //_visualBootStrapper.StyleContext.RegisterStyleSetter(btn, StyleSetter.Padding,
            //    StyleSelector.None, new Thickness(15, 5, 15, 15));
            _visualBootstrapper.StyleContext.RegisterStyleSetter(btn, StyleSetter.FontSize,
                StyleSelector.None, 15);

            btn.VerticalAlignment = VerticalAlignments.Default;
            btn.HorizontalAlignment = HorizontalAlignments.Default;

            return btn;
        }
    }
}

using System;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Styles;

namespace Das.Views.Defaults
{
    public class DefaultTabItemTemplate : DefaultContentTemplate
    {
        //private readonly ITabControl<TDataContext> _tabControl;


        public DefaultTabItemTemplate(IVisualBootstrapper visualBootstrapper)
                                      //ITabControl<TDataContext> tabControl) 
            : base(visualBootstrapper)
        {
          //  _tabControl = tabControl;
        }

        //public override TVisualElement BuildVisual<TVisualElement>(TDataContext dataContext)
        //{
        //    return ((DefaultContentTemplate) this).BuildVisual<TVisualElement>(dataContext);
        //}


        public override IVisualElement BuildVisual(Object? dataContext)
        {
            var btn = new ToggleButton(_visualBootstrapper)
            {
                DataContext = dataContext
            };
            _visualBootstrapper.StyleContext.RegisterStyleSetter(btn, StyleSetterType.BorderBrush,
                SolidColorBrush.Tranparent);
            _visualBootstrapper.StyleContext.RegisterStyleSetter(btn, StyleSetterType.Background,
                VisualStateType.Active, SolidColorBrush.Tranparent);
            _visualBootstrapper.StyleContext.RegisterStyleSetter(btn, StyleSetterType.Background,
                VisualStateType.Checked, SolidColorBrush.Tranparent);


            _visualBootstrapper.StyleContext.RegisterStyleSetter(btn, StyleSetterType.Background,
                VisualStateType.None, SolidColorBrush.White);

            //btn.Width = 80;
            //btn.Height = 50;
            //_visualBootStrapper.StyleContext.RegisterStyleSetter(btn, StyleSetter.Width,
            //    StyleSelector.None, 150);
            //_visualBootStrapper.StyleContext.RegisterStyleSetter(btn, StyleSetter.Padding,
            //    StyleSelector.None, new Thickness(15, 5, 15, 15));
            _visualBootstrapper.StyleContext.RegisterStyleSetter(btn, StyleSetterType.FontSize,
                VisualStateType.None, 15);

            btn.VerticalAlignment = VerticalAlignments.Default;
            btn.HorizontalAlignment = HorizontalAlignments.Default;

            return btn;
        }
    }
}

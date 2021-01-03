using System;
using Das.Views.Controls;
using Das.Views.Core.Enums;

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
                FontSize = 15,
                DataContext = dataContext
            };





            //_visualBootstrapper.StyleContext.RegisterStyleSetter(btn, StyleSetterType.BorderBrush,
            //    SolidColorBrush.Tranparent);
            
            //_visualBootstrapper.StyleContext.RegisterStyleSetter(btn, StyleSetterType.Background,
            //    VisualStateType.Active, SolidColorBrush.Tranparent);
            //_visualBootstrapper.StyleContext.RegisterStyleSetter(btn, StyleSetterType.Background,
            //    VisualStateType.Checked, SolidColorBrush.Tranparent);


            //_visualBootstrapper.StyleContext.RegisterStyleSetter(btn, StyleSetterType.Background,
            //    VisualStateType.None, SolidColorBrush.White);

            
            //_visualBootstrapper.StyleContext.RegisterStyleSetter(btn, StyleSetterType.FontSize,
            //    VisualStateType.None, 15);

            btn.VerticalAlignment = VerticalAlignments.Default;
            btn.HorizontalAlignment = HorizontalAlignments.Default;

            return btn;
        }
    }
}

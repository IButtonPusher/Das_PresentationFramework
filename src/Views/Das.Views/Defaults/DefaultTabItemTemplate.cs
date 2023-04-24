using System;
using System.Collections.Generic;
using Das.Views.BoxModel;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Styles;
using Das.Views.Styles.Declarations;
using Das.Views.Styles.Selectors;

namespace Das.Views.Defaults;

public class DefaultTabItemTemplate : DefaultContentTemplate
{
   public DefaultTabItemTemplate(IVisualBootstrapper visualBootstrapper)
      : base(visualBootstrapper)
   {
      _buttonStyle = new StyleSheet(GetStyleRules());
   }

   private static IEnumerable<IStyleRule> GetStyleRules()
   {
      var svr = Rule<IBrush?>(VisualStateType.Active, SolidColorBrush.LightGray,
         DeclarationProperty.BackgroundColor);
      yield return svr;
   }

   private static IStyleRule Rule<TValue>(VisualStateType state,
                                          TValue value,
                                          DeclarationProperty declarationProperty)
   {
      return new StyleValueRule(Select(state),
         new List<IStyleDeclaration>
         {
            new ValueDeclaration<TValue>(value, /*_variableAccessor, */declarationProperty)
         });
   }

   private static VisualStateSelector Select(VisualStateType state)
   {
      return new(AllStyleSelector.Instance, state);
   }

   public override IVisualElement BuildVisual(Object? dataContext)
   {
      var btn = _visualBootstrapper.Instantiate<RadioButton>();
            
      //_visualBootstrapper.ApplyVisualStyling(btn);
      //_visualBootstrapper.ApplyVisualStyling(btn, _buttonStyle);
      btn.FontSize = 15;
      btn.Background = default;
      btn.DataContext = dataContext;
      btn.Padding = new QuantifiedThickness(5, 10, 5, 10);
      btn.Border = VisualBorder.Empty;

      btn.VerticalAlignment = VerticalAlignments.Default;
      btn.HorizontalAlignment = HorizontalAlignments.Default;

      //btn.Style

      return btn;
   }

   private StyleSheet _buttonStyle;
}
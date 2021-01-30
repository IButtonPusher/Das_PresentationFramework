using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.BoxModel;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Panels;
using Das.Views.Styles.Declarations;
using Das.Views.Styles.Selectors;

namespace Das.Views.Styles.DefaultStyles
{
    public class BasePrimitiveStyle : Dictionary<Type, IEnumerable<IStyleRule>>
    {
        public BasePrimitiveStyle(IStyleVariableAccessor variableAccessor)
        {
            _variableAccessor = variableAccessor;
            var btnBaseItems = new List<IStyleRule>
            {
                Rule(VisualElement.BorderRadiusProperty, 8, DeclarationProperty.BorderRadius),
                Rule(VisualElement.BorderProperty, new VisualBorder(
                        new BorderSide(1, LengthUnits.Px, OutlineStyle.Solid, SolidColorBrush.Black)),
                    DeclarationProperty.Border),

                Rule(ContentPanel.PaddingProperty, 5, DeclarationProperty.Padding)
            };

            this[typeof(ButtonBase)] = btnBaseItems;

            this[typeof(ToggleButton)] = new[]
            {
                Rule(VisualElement.BackgroundProperty, VisualStateType.Checked, SolidColorBrush.LightGray,
                    DeclarationProperty.BackgroundColor),
                Rule(VisualElement.BackgroundProperty, VisualStateType.Active, SolidColorBrush.LightGray,
                    DeclarationProperty.BackgroundColor)
            };
        }

        private IStyleRule Rule<TValue>(IDependencyProperty<TValue> property,
                                        TValue value,
                                        DeclarationProperty declarationProperty)
        {
            return new DependencyPropertyValueRule<TValue>(
                AllStyleSelector.Instance,
                //Select(property),
                new ValueDeclaration<TValue>(value, _variableAccessor, declarationProperty));
        }

        private IStyleRule Rule<TValue>(IDependencyProperty<TValue> property,
                                        VisualStateType state,
                                        TValue value,
                                        DeclarationProperty declarationProperty)
        {
            return new StyleValueRule(Select(state, property),
                new List<IStyleDeclaration>
                {
                    new ValueDeclaration<TValue>(value, _variableAccessor, declarationProperty)
                });
        }


        private static VisualStateSelector Select<TValue>(VisualStateType state,
                                                          IDependencyProperty<TValue> property)
        {
            return new VisualStateSelector(AllStyleSelector.Instance, state);
            //return new VisualStateSelector(Select(property), state);
        }

        //private static IStyleSelector Select<TValue>(IDependencyProperty<TValue> property)
        //{
        //    return AllStyleSelector.Instance;
        //    //return new DependencyPropertySelector(property);
        //}

        private readonly IStyleVariableAccessor _variableAccessor;
    }
}

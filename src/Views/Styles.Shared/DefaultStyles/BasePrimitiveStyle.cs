using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.BoxModel;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Styles.Declarations;
using Das.Views.Styles.Selectors;

namespace Das.Views.Styles
{
    public class BasePrimitiveStyle : Dictionary<Type, IEnumerable<IStyleRule>>
    {
        public BasePrimitiveStyle(IStyleVariableAccessor variableAccessor)
        {
            _variableAccessor = variableAccessor;
            var btnBaseItems = new List<IStyleRule>
            {
                Rule<QuantifiedThickness>(8, DeclarationProperty.BorderRadius),
                Rule(new VisualBorder(
                        new BorderSide(1, LengthUnits.Px, OutlineStyle.Solid, SolidColorBrush.Black)),
                    DeclarationProperty.Border),

                Rule<QuantifiedThickness>(5, DeclarationProperty.Padding),
                Rule<IBrush?>(VisualStateType.Active, SolidColorBrush.LightGray,
                    DeclarationProperty.BackgroundColor)
            };

            this[typeof(ButtonBase)] = btnBaseItems;

            this[typeof(ToggleButton)] = new[]
            {
                Rule<IBrush?>(VisualStateType.Checked, SolidColorBrush.LightGray,
                    DeclarationProperty.BackgroundColor),
                Rule<IBrush?>(VisualStateType.Active, SolidColorBrush.LightGray,
                    DeclarationProperty.BackgroundColor)
            };
        }

        private IStyleRule Rule<TValue>(TValue value,
                                        DeclarationProperty declarationProperty)
        {
            return new DependencyPropertyValueRule<TValue>(
                AllStyleSelector.Instance,
                new ValueDeclaration<TValue>(value, _variableAccessor, declarationProperty));
        }

        private IStyleRule Rule<TValue>(VisualStateType state,
                                        TValue value,
                                        DeclarationProperty declarationProperty)
        {
            return new StyleValueRule(Select(state),
                new List<IStyleDeclaration>
                {
                    new ValueDeclaration<TValue>(value, _variableAccessor, declarationProperty)
                });
        }


        private static VisualStateSelector Select(VisualStateType state)
        {
            return new(AllStyleSelector.Instance, state);
        }

        private readonly IStyleVariableAccessor _variableAccessor;
    }
}

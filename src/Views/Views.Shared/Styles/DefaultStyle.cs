using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public sealed class DefaultStyle : StyleBase, 
                                       IStyleSheet
    {
        public static readonly DefaultStyle Instance = new DefaultStyle();
        
        private DefaultStyle()
        {
            var font = new Font(10, FontName, FontStyle.Regular);

            this[StyleSetterType.Margin] = new Thickness(0);
            this[StyleSetterType.Padding] = new Thickness(0);

            this[StyleSetterType.BorderThickness] = new Thickness(0);
            this[StyleSetterType.BorderRadius] = 0;

            this[StyleSetterType.BorderBrush] = new SolidColorBrush(Color.Transparent);
            this[StyleSetterType.Foreground] = new SolidColorBrush(Color.Black);
            this[StyleSetterType.Background] = new SolidColorBrush(Color.Transparent);

            this[StyleSetterType.FontName] = font.FamilyName;
            this[StyleSetterType.FontSize] = font.Size;
            this[StyleSetterType.FontWeight] = font.FontStyle;
            this[StyleSetterType.Font] = font;
            this[StyleSetterType.HorizontalAlignment] = HorizontalAlignments.Default;
            this[StyleSetterType.VerticalAlignment] = VerticalAlignments.Default;
            
            this[StyleSetterType.Height] = Double.NaN;
            this[StyleSetterType.Width] = Double.NaN;
            this[StyleSetterType.Visibility] = Visibility.Visible;
            this[StyleSetterType.Transition] = Transition.EmptyTransitions;
            this[StyleSetterType.ZIndex] = 0;

            this[StyleSetterType.Template] = default;

            var typeTypes = new Dictionary<Type, IStyleSheet>
            {
                [typeof(IButtonBase)] = new TypeStyle<IButtonBase>
                {
                    {StyleSetterType.BorderRadius, VisualStateType.None, 8},
                    {StyleSetterType.BorderThickness, VisualStateType.None, 1},
                    {StyleSetterType.BorderBrush, VisualStateType.None, SolidColorBrush.Black},
                    {StyleSetterType.Background, VisualStateType.Active, SolidColorBrush.LightGray},
                    {StyleSetterType.Background, VisualStateType.Hover, SolidColorBrush.Pink},
                    {StyleSetterType.Padding, VisualStateType.None, new Thickness(5)}
                },
                [typeof(IToggleButton)] = new TypeStyle<IToggleButton>
                {
                    {StyleSetterType.Background, VisualStateType.Checked, SolidColorBrush.LightGray},
                }
            };
            VisualTypeStyles = typeTypes;
        }

        public IEnumerable<IStyleRule> Rules => Enumerable.Empty<IStyleRule>();

        public IDictionary<Type, IStyleSheet> VisualTypeStyles { get; }

        public IEnumerable<IStyleSetter> StyleSetters
        {
            get => StyleSheetHelper.GetAllSetters(this);
        }


        public const String FontName = "Segoe UI";

        public Object? this[StyleSetterType setterType]
        {
            get => TryGetValue(setterType, VisualStateType.None, out var found)
                ? found
                : default;
            private set => AddSetterImpl(setterType, value);
        }

        public Object? this[StyleSetterType setterType,
                                    VisualStateType type]
        {
            get => TryGetValue(setterType, type, out var found)
                ? found
                : default;
            //private set => AddImpl(setter, selector, value);
        }


        void IStyle.Add(StyleSetterType setterType, 
                        VisualStateType type, 
                        Object? value)
        {
            throw new NotSupportedException();
        }

        //void IStyle.AddOrUpdate(IStyle style)
        //{
        //    throw new NotSupportedException();
        //}

        //void IStyle.AddSetter(StyleSetterType setterType, Object? value)
        //{
        //    throw new NotSupportedException();
        //}
        
        //protected readonly Dictionary<AssignedStyle, Object?> _setters;
    }
}
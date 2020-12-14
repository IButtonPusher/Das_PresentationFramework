﻿using System;
using System.Collections.Generic;
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
    public sealed class DefaultStyle : StyleBase, //Style, 
                                IStyleSheet
    {
        public static DefaultStyle Instance = new DefaultStyle();
        
        private DefaultStyle()
        {
            //_setters = new Dictionary<AssignedStyle, Object?>()
            
            var font = new Font(10, FontName, FontStyle.Regular);

            this[StyleSetter.Margin] = new Thickness(0);
            this[StyleSetter.Padding] = new Thickness(0);

            this[StyleSetter.BorderThickness] = new Thickness(0);
            this[StyleSetter.BorderRadius] = 0;

            this[StyleSetter.BorderBrush] = new SolidColorBrush(Color.Transparent);
            this[StyleSetter.Foreground] = new SolidColorBrush(Color.Black);
            this[StyleSetter.Background] = new SolidColorBrush(Color.Transparent);

            this[StyleSetter.FontName] = font.FamilyName;
            this[StyleSetter.FontSize] = font.Size;
            this[StyleSetter.FontWeight] = font.FontStyle;
            this[StyleSetter.Font] = font;
            this[StyleSetter.HorizontalAlignment] = HorizontalAlignments.Default;
            this[StyleSetter.VerticalAlignment] = VerticalAlignments.Default;
            //this[StyleSetter.Size] = null;
            this[StyleSetter.Height] = Double.NaN;
            this[StyleSetter.Width] = Double.NaN;
            this[StyleSetter.Visibility] = Visibility.Visible;
            this[StyleSetter.Transition] = Transition.EmptyTransitions;

            var typeTypes = new Dictionary<Type, IStyle>
            {
                [typeof(IButtonBase)] = new TypeStyle<IButtonBase>
                {
                    {StyleSetter.BorderRadius, StyleSelector.None, 8},
                    {StyleSetter.BorderThickness, StyleSelector.None, 1},
                    {StyleSetter.BorderBrush, StyleSelector.None, SolidColorBrush.Black},
                    {StyleSetter.Background, StyleSelector.Active, SolidColorBrush.LightGray},
                    {StyleSetter.Background, StyleSelector.Hover, SolidColorBrush.Pink},
                    {StyleSetter.Padding, StyleSelector.None, new Thickness(5)}
                },
                [typeof(IToggleButton)] = new TypeStyle<IToggleButton>
                {
                    {StyleSetter.Background, StyleSelector.Checked, SolidColorBrush.LightGray},
                }
            };
            VisualTypeStyles = typeTypes;
        }

        public IDictionary<Type, IStyle> VisualTypeStyles { get; }

        public const String FontName = "Segoe UI";

        public Object? this[StyleSetter setter]
        {
            get => TryGetValue(setter, StyleSelector.None, out var found)
                ? found
                : default;
            private set => AddSetterImpl(setter, value);
        }

        public Object? this[StyleSetter setter,
                                    StyleSelector selector]
        {
            get => TryGetValue(setter, selector, out var found)
                ? found
                : default;
            private set => AddImpl(setter, selector, value);
        }


        void IStyle.Add(StyleSetter setter, 
                        StyleSelector selector, 
                        Object? value)
        {
            throw new NotSupportedException();
        }

        void IStyle.AddOrUpdate(IStyle style)
        {
            throw new NotSupportedException();
        }

        void IStyle.AddSetter(StyleSetter setter, Object? value)
        {
            throw new NotSupportedException();
        }
        
        //protected readonly Dictionary<AssignedStyle, Object?> _setters;
    }
}
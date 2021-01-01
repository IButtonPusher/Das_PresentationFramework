﻿using System;

namespace Das.Views.Styles.Selectors
{
    public class DependencyPropertySelector : SelectorBase
    {
        public DependencyPropertySelector(IDependencyProperty property)
        {
            Property = property;
        }

        public IDependencyProperty Property {get;}

        public sealed override Boolean Equals(IStyleSelector other)
        {
            return other is DependencyPropertySelector depSel &&
                   depSel.Property.Name == Property.Name &&
                   depSel.Property.VisualType == Property.VisualType &&
                   depSel.Property.PropertyType == Property.PropertyType;

        }
    }
}
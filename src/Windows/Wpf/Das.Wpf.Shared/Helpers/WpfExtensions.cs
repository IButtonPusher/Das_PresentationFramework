using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Das.Views.Wpf.Helpers;

public static class WpfExtensions
{
    public static IEnumerable<TVisual> EnumVisual<TVisual>(this DependencyObject myVisual)
        where TVisual : FrameworkElement
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(myVisual); i++)
        {
            // Retrieve child visual at specified index value.

            var nowChild = VisualTreeHelper.GetChild(myVisual, i);
            if (VisualTreeHelper.GetChild(myVisual, i) is TVisual childVisual)
                yield return childVisual;

            foreach (var grandChild in nowChild.EnumVisual<TVisual>())
            {
                yield return grandChild;
            }
        }
    }

    public static IEnumerable<TChild> GetTree<TChild>(this DependencyObject reference)
        where TChild : DependencyObject
    {
        if (reference == null!)
            yield break;

        var childrens = GetDependents(reference);

        foreach (var child in childrens)
        {
            if (child is TChild tc)
                yield return tc;

            foreach (var moChild in GetTree<TChild>(child))
            {
                yield return moChild;
            }
        }
    }

    public static Size MeasureText(this TextBlock tb,
                                   String text,
                                   Double fontSize)
    {
        var formattedText = new FormattedText(
            text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface(tb.FontFamily, tb.FontStyle,
                tb.FontWeight, tb.FontStretch),
            fontSize,
            Brushes.Black,
            new NumberSubstitution(),
            1);

        return new Size(formattedText.Width, formattedText.Height);
    }

    public static Size MeasureText(this TextBlock tb,
                                   Double fontSize)
        => tb.MeasureText(tb.Text, fontSize);

    public static Size MeasureText(this TextBlock tb)
        => tb.MeasureText(tb.Text, tb.FontSize);

    private static IEnumerable<DependencyObject> GetDependents(DependencyObject reference)
    {
        var childrenCount = reference is Visual v ? VisualTreeHelper.GetChildrenCount(v) : 0;


        if (childrenCount == 0)
        {
            if (reference is ContentControl ctrl &&
                ctrl.Content is DependencyObject dep)
                yield return dep;

            else if (reference is ItemsControl items)
                foreach (var item in items.Items.OfType<DependencyObject>())
                {
                    yield return item;
                }

            else
            {
                var attribs = reference.GetType()
                                       .GetCustomAttributes(true)
                                       .OfType<ContentPropertyAttribute>()
                                       .FirstOrDefault();
                if (attribs != null && reference.TryGetPropertyValue<DependencyObject>(attribs.Name,
                        out var content))
                {
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    if (content is IEnumerable enumerable)
                        foreach (var item in enumerable.OfType<DependencyObject>())
                        {
                            yield return item;
                        }
                    else if (content is { } recursive)
                        foreach (var distantChild in GetDependents(recursive))
                        {
                            yield return distantChild;
                        }
                }
            }
        }

        for (var i = 0; i < childrenCount; i++) yield return VisualTreeHelper.GetChild(reference, i);
    }

    private static Boolean TryGetPropertyValue<TValue>(this DependencyObject obj,
                                                       String propertyName,
                                                       out TValue value)
    {
        var otype = obj.GetType();
        if (!_contentProperties.TryGetValue(otype, out var prop))
        {
            prop = otype.GetProperty(propertyName, BindingFlags.FlattenHierarchy |
                                                   BindingFlags.Public |
                                                   BindingFlags.NonPublic);
            _contentProperties.Add(otype, prop);
        }

        if (prop is { } valid &&
            valid.GetValue(obj) is TValue good)
        {
            value = good;
            return true;
        }

        value = default!;
        return false;
    }

    private static readonly Dictionary<Type, PropertyInfo?> _contentProperties = new();
}

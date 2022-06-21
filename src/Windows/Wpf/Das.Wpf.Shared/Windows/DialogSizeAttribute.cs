using System;
using System.Threading.Tasks;

namespace Das.Views.Wpf.Windows;

public class DialogSizeAttribute : Attribute
{
    public DialogSizeAttribute(Double width,
                               Double height)
    {
        Width = width;
        Height = height;
    }

    public Double Width { get; }

    public Double Height { get; }
}

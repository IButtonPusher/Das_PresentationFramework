using System;
using System.Threading.Tasks;

namespace Das.Views.Wpf.Windows;

public class WindowDisplayAttribute : Attribute
{
    public WindowDisplayAttribute(Double width,
                               Double height)
    {
        Width = width;
        Height = height;
    }

    public WindowDisplayAttribute(Double width,
                                  Double height,
                                  Boolean showInTaskBar)
    : this(width, height)
    {
       ShowInTaskBar = showInTaskBar;
    }

    public Double Width { get; }

    public Double Height { get; }

    public Boolean? ShowInTaskBar { get; }
}

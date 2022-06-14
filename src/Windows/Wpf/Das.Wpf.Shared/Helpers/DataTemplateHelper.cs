using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Das.Views.Wpf.Helpers;

public static class DataTemplateHelper
{
    public static TVisual GetVisualFromTemplate<TDataContext, TVisual>()
        where TVisual : UIElement, new()
    {
        return GetVisualFromTemplate<TDataContext, TVisual>(Application.Current.Resources);
    }

    public static DataTemplate GetDataTemplate<TDataContext>()
        => GetDataTemplate<TDataContext>(Application.Current.Resources);

    public static DataTemplate GetDataTemplate<TDataContext>(ResourceDictionary resources)
    {
        var k = new DataTemplateKey(typeof(TDataContext));
        if (resources[k] is not DataTemplate dataTemplate)
            throw new ResourceReferenceKeyNotFoundException("No template was found",
                $"{typeof(TDataContext)}");

        return dataTemplate;
    }

    public static TVisual GetVisualFromTemplate<TDataContext, TVisual>(ResourceDictionary resources)
        where TVisual : UIElement, new()
    {
        var dataTemplate = GetDataTemplate<TDataContext>(resources);
        //var k = new DataTemplateKey(typeof(TDataContext));
        //if (resources[k] is not DataTemplate dataTemplate)
        //    throw new ResourceReferenceKeyNotFoundException("No template was found",
        //        $"{typeof(TDataContext)}");


        var contents = dataTemplate.LoadContent();

        switch (contents)
        {
            case TVisual visual:
                return visual;

            case UIElement ui when typeof(ContentControl).IsAssignableFrom(typeof(TVisual)):

                var res = new TVisual();

                if (res is ContentControl cc)
                    cc.Content = ui;

                return res;

            case { } dep:
                throw new InvalidCastException($"{dep.GetType()} cannot be cast to {typeof(TVisual)}");

            default:
                throw new ResourceReferenceKeyNotFoundException("No template was found",
                    $"{typeof(TDataContext)}");
        }
    }
}

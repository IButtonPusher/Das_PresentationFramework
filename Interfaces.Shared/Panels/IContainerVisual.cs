using System;
using System.ComponentModel;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public interface IContainerVisual : IVisualElement,
                                        //IChangeTracking,
                                        INotifyPropertyChanged
    {
    }
}

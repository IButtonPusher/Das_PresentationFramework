using System;
using System.ComponentModel;

namespace Das.Views.Panels
{
    public interface IContainerVisual : IVisualElement,
                                        //IChangeTracking,
                                        INotifyPropertyChanged
    {
    }
}

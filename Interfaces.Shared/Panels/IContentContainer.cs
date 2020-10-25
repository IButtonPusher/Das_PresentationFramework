using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public interface IContentContainer : IVisualElement, 
                                         IChangeTracking,
                                         INotifyPropertyChanged
    {
        IVisualElement? Content { get; set; }
    }
}
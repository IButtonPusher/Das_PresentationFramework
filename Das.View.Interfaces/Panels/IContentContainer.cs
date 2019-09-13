using System;
using System.ComponentModel;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public interface IContentContainer : IVisualElement, IChangeTracking
    {
        IVisualElement Content { get; set; }
    }
}
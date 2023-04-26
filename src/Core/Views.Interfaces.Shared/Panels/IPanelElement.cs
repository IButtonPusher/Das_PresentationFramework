using System;
using System.Collections.Generic;

namespace Das.Views.Panels;

/// <summary>
/// Convenience interface for runtime visual cloning/inflating where type is
/// not easily determined
/// </summary>
public interface IPanelElement : IVisualElement
{
   IVisualCollection Children { get; }

   void AddChild(IVisualElement element);
        
   void AddChildren(IEnumerable<IVisualElement> elements);
}
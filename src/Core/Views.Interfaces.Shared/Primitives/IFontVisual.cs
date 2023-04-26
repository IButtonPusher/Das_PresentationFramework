using System;
using Das.Views.Core.Writing;

namespace Das.Views.Primitives;

public interface IFontVisual : IVisualElement
{
   FontStyle FontWeight { get; set; }

   String FontName { get; set; }

   Double FontSize { get; set; }
}
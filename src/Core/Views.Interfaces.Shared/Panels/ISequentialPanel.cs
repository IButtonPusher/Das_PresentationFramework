using System;
using System.Threading.Tasks;
using Das.Views.Core.Enums;

namespace Das.Views.Panels;

public interface ISequentialPanel : IVisualElement //: IVisualContainer
{
   IVisualCollection Children { get; }

   Orientations Orientation { get; set; }
}
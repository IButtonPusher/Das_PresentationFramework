using System;
using System.Threading.Tasks;
using Das.Views.Core.Enums;

namespace Das.Views.Panels
{
    public interface ISequentialPanel : IVisualContainer
    {
        Orientations Orientation { get; set; }
    }
}
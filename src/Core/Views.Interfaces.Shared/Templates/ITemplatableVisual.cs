using System;
using Das.Views.Controls;

namespace Das.Views.Templates
{
    public interface ITemplatableVisual
    {
        IVisualTemplate? Template { get; }
    }
}

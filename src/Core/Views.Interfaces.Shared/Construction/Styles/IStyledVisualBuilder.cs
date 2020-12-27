﻿using System;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views.Construction.Styles
{
    public interface IStyledVisualBuilder
    {
        Task ApplyStylesToVisualAsync(IVisualElement visual,
                                 String? styleClassName,
                                 IVisualLineage visualLineage);

        Task ApplyStylesToVisualAsync(IVisualElement visual,
                                      IAttributeDictionary attributeDictionary,
                                      IVisualLineage visualLineage,
                                      IViewInflater viewInflater);
    }
}

using System;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public interface IStyleProvider
    {
        T GetStyleSetter<T>(StyleSetter setter, 
                            IVisualElement element);

        /// <summary>
        /// Falls back to StyleSelector.None if a different value is passed and no setter is found
        /// </summary>
        T GetStyleSetter<T>(StyleSetter setter, 
                            StyleSelector selector,
                            IVisualElement element);
    }
}
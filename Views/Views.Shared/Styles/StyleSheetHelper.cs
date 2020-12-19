using System;
using System.Collections.Generic;

namespace Das.Views.Styles
{
    public static class StyleSheetHelper
    {
        public static IEnumerable<IStyleSetter> GetAllSetters(IStyleSheet styleSheet)
        {
            foreach (var item in styleSheet)
            {
                yield return item;
            }

            foreach (var kvp in styleSheet.VisualTypeStyles)
            {
                foreach (var childSetter in kvp.Value.StyleSetters)
                    yield return childSetter;
            }
        }
    }
}

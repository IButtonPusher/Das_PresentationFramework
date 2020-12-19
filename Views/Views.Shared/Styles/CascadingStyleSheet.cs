using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Das.Views.Styles
{
    [ContentProperty(nameof(StyleSetters))]
    public class CascadingStyleSheet : Style,
                                       IStyleSheet
    {
        public CascadingStyleSheet()
        {
            VisualTypeStyles = new ConcurrentDictionary<Type, IStyleSheet>();
        }
        public IDictionary<Type, IStyleSheet> VisualTypeStyles { get; }
        
        public IEnumerable<IStyleSetter> StyleSetters
        {
            get => StyleSheetHelper.GetAllSetters(this);
            set => UpdateSetters(value);
        }

        //protected IEnumerable<IStyleSetter> GetAllSetters()
        //{
        //    foreach (var item in _setters)
        //    {
        //        yield return item.Key;
        //    }

        //    foreach (var kvp in VisualTypeStyles)
        //    {
        //        foreach (var childSetter in kvp.Value.StyleSetters)
        //            yield return childSetter;
        //    }
        //}

        protected void UpdateSetters(IEnumerable<IStyleSetter> setters)
        {
            _setters.Clear();
            VisualTypeStyles.Clear();

            throw new NotImplementedException();
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Styles
{
    public interface IStyle : IEnumerable<KeyValuePair<StyleSetters, Object?>>
    {
        Object? this[StyleSetters setter] { get; }

        Boolean TryGetValue(StyleSetters setter, out Object val);

        Boolean TryGetValue(StyleSetters setter, 
                            Object dataContext, 
                            out Object val);
    }
}